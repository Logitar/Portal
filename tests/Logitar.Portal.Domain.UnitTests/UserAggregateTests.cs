using Bogus;
using Logitar.EventSourcing;
using Logitar.Portal.Domain.Passwords;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Settings;
using Logitar.Portal.Domain.Users;
using Logitar.Security.Cryptography;

namespace Logitar.Portal.Domain;

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
    string secretString = RandomStringGenerator.GetString(32);
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
}
