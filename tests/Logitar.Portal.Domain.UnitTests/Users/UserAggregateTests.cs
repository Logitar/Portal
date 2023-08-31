using Bogus;
using Logitar.EventSourcing;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Settings;
using Logitar.Security.Cryptography;

namespace Logitar.Portal.Domain.Users;

[Trait(Traits.Category, Categories.Unit)]
public class UserAggregateTests
{
  private const string PasswordString = "Test123!";

  private readonly Faker _faker = new();

  private readonly UserAggregate _user;
  private readonly IUserSettings _userSettings;

  public UserAggregateTests()
  {
    ReadOnlyUniqueNameSettings uniqueNameSettings = new();
    ReadOnlyPasswordSettings passwordSettings = new();
    _userSettings = new ReadOnlyUserSettings(requireUniqueEmail: true, requireConfirmedAccount: true, uniqueNameSettings, passwordSettings);

    _user = new(_userSettings.UniqueNameSettings, _faker.Person.UserName);
  }

  [Fact(DisplayName = "ChangePassword: it should change the user password.")]
  public void ChangePassword_it_should_change_the_user_password()
  {
    string currentPassword = string.Concat(PasswordString, '*');
    PasswordMock password = new(currentPassword);
    _user.SetPassword(password);

    ActorId actorId = ActorId.NewId();
    PasswordMock newPassword = new(PasswordString);
    _user.ChangePassword(currentPassword, newPassword, actorId);

    Assert.Equal(actorId, _user.UpdatedBy);
    AssertUserPassword();
  }

  [Fact(DisplayName = "ChangePassword: it should throw IncorrectUserPasswordException when the current password is incorrect.")]
  public void ChangePassword_it_should_throw_IncorrectUserPasswordException_when_the_current_password_is_incorrect()
  {
    string currentPassword = string.Concat(PasswordString, '*');
    PasswordMock oldPassword = new(PasswordString);
    _user.SetPassword(oldPassword);

    PasswordMock newPassword = new(currentPassword);
    var exception = Assert.Throws<IncorrectUserPasswordException>(() => _user.ChangePassword(currentPassword, newPassword));
    Assert.Equal(_user.ToString(), exception.User);
    Assert.Equal(currentPassword, exception.Password);
  }

  [Fact(DisplayName = "ChangePassword: it should throw IncorrectUserPasswordException when the user has no password.")]
  public void ChangePassword_it_should_throw_IncorrectUserPasswordException_when_the_user_has_no_password()
  {
    PasswordMock newPassword = new(PasswordString);
    var exception = Assert.Throws<IncorrectUserPasswordException>(() => _user.ChangePassword(PasswordString, newPassword));
    Assert.Equal(_user.ToString(), exception.User);
    Assert.Equal(PasswordString, exception.Password);
  }

  [Fact(DisplayName = "ChangePassword: it should use the user ID as actor ID.")]
  public void ChangePassword_it_should_use_the_user_Id_as_actor_Id()
  {
    string currentPassword = string.Concat(PasswordString, '*');
    PasswordMock password = new(currentPassword);
    _user.SetPassword(password);

    ActorId actorId = new(_user.Id.Value);
    PasswordMock newPassword = new(PasswordString);
    _user.ChangePassword(currentPassword, newPassword, actorId);

    Assert.Equal(actorId, _user.UpdatedBy);
  }

  [Fact(DisplayName = "SignIn: it should issue a session when the password is correct.")]
  public void SignIn_it_should_issue_a_session_when_the_password_is_correct()
  {
    ActorId actorId = new(_user.Id.Value);

    _user.Email = new EmailAddress(_faker.Person.Email, isVerified: true);
    _user.SetPassword(new PasswordMock(PasswordString));
    SessionAggregate session = _user.SignIn(_userSettings, PasswordString);

    Assert.Equal(actorId, session.CreatedBy);
    Assert.Equal(_user.AuthenticatedOn, session.CreatedOn);
    Assert.Equal(actorId, session.UpdatedBy);
    Assert.Equal(_user.AuthenticatedOn, session.UpdatedOn);
    Assert.Equal(1, session.Version);

    Assert.Equal(_user.Id, session.UserId);
    Assert.False(session.IsPersistent);
    Assert.True(session.IsActive);

    Assert.True((DateTime.Now - _user.AuthenticatedOn) < TimeSpan.FromMinutes(1));
    Assert.Equal(actorId, _user.UpdatedBy);
    Assert.Equal(_user.AuthenticatedOn, _user.UpdatedOn);
    Assert.True(_user.Version > 1);
  }

  [Fact(DisplayName = "SignIn: it should issue a session without a password.")]
  public void SignIn_it_should_issue_a_session_without_a_password()
  {
    ActorId actorId = ActorId.NewId();

    ReadOnlyUserSettings userSettings = new();
    string secretString = RandomStringGenerator.GetString(SessionAggregate.SecretLength);
    PasswordMock secret = new(secretString);
    SessionAggregate session = _user.SignIn(userSettings, secret, actorId);

    Assert.Equal(actorId, session.CreatedBy);
    Assert.Equal(_user.AuthenticatedOn, session.CreatedOn);
    Assert.Equal(actorId, session.UpdatedBy);
    Assert.Equal(_user.AuthenticatedOn, session.UpdatedOn);
    Assert.Equal(1, session.Version);

    Assert.Equal(_user.Id, session.UserId);
    Assert.True(session.IsPersistent);
    Assert.True(session.IsActive);

    Assert.True((DateTime.Now - _user.AuthenticatedOn) < TimeSpan.FromMinutes(1));
    Assert.Equal(actorId, _user.UpdatedBy);
    Assert.Equal(_user.AuthenticatedOn, _user.UpdatedOn);
    Assert.True(_user.Version > 1);

    FieldInfo? secretField = session.GetType().GetField("_secret", BindingFlags.Instance | BindingFlags.NonPublic);
    Assert.NotNull(secretField);
    Password? secretValue = secretField.GetValue(session) as Password;
    Assert.NotNull(secretValue);
    Assert.True(secretValue.IsMatch(secretString));
  }

  [Fact(DisplayName = "SignIn: it should throw IncorrectUserPasswordException when password is incorrect.")]
  public void SignIn_it_should_throw_IncorrectUserPasswordException_when_password_is_incorrect()
  {
    _user.SetPassword(new PasswordMock(PasswordString[1..]));

    var exception = Assert.Throws<IncorrectUserPasswordException>(() => _user.SignIn(_userSettings, PasswordString));
    Assert.Equal(_user.ToString(), exception.User);
    Assert.Equal(PasswordString, exception.Password);
  }

  [Fact(DisplayName = "SignIn: it should throw IncorrectUserPasswordException when user has no password.")]
  public void SignIn_it_should_throw_IncorrectUserPasswordException_when_user_has_no_password()
  {
    var exception = Assert.Throws<IncorrectUserPasswordException>(() => _user.SignIn(_userSettings, PasswordString));
    Assert.Equal(_user.ToString(), exception.User);
    Assert.Equal(PasswordString, exception.Password);
  }

  [Fact(DisplayName = "SignIn: it should throw UserIsDisabledException when user is disabled.")]
  public void SignIn_it_should_throw_UserIsDisabledException_when_user_is_disabled()
  {
    _user.Disable();
    var exception = Assert.Throws<UserIsDisabledException>(() => _user.SignIn(_userSettings));
    Assert.Equal(_user.ToString(), exception.User);
  }

  [Fact(DisplayName = "SignIn: it should throw UserIsNotConfirmedException when user is not confirmed.")]
  public void SignIn_it_should_throw_UserIsNotConfirmedException_when_user_is_not_confirmed()
  {
    var exception = Assert.Throws<UserIsNotConfirmedException>(() => _user.SignIn(_userSettings));
    Assert.Equal(_user.ToString(), exception.User);
  }

  private void AssertUserPassword(UserAggregate? user = null, string? password = null)
  {
    user ??= _user;
    password ??= PasswordString;

    FieldInfo? passwordField = user.GetType().GetField("_password", BindingFlags.Instance | BindingFlags.NonPublic);
    Assert.NotNull(passwordField);

    Password? instance = (Password?)passwordField.GetValue(user);
    Assert.NotNull(instance);
    Assert.True(instance.IsMatch(password));
  }
}
