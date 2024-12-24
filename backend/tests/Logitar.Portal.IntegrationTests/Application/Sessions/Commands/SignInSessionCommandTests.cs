using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Passwords;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Sessions.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class SignInSessionCommandTests : IntegrationTests
{
  private readonly IPasswordManager _passwordManager;
  private readonly IUserRepository _userRepository;

  public SignInSessionCommandTests() : base()
  {
    _passwordManager = ServiceProvider.GetRequiredService<IPasswordManager>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should create a persistent session.")]
  public async Task It_should_create_a_persistent_session()
  {
    CustomAttribute[] customAttributes =
    [
      new("AdditionalInformation", $@"{{""User-Agent"":""{Faker.Internet.UserAgent()}""}}"),
      new("IpAddress", Faker.Internet.Ip())
    ];
    SignInSessionPayload payload = new(UsernameString, PasswordString, isPersistent: true, customAttributes);
    SignInSessionCommand command = new(payload);
    SessionModel session = await ActivityPipeline.ExecuteAsync(command);

    Assert.True(session.IsPersistent);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Equal(customAttributes, session.CustomAttributes);
    Assert.Equal(UsernameString, session.User.UniqueName);

    Assert.NotNull(session.RefreshToken);
    RefreshToken refreshToken = RefreshToken.Decode(session.RefreshToken);
    Assert.Equal(session.Id, refreshToken.Id.EntityId.ToGuid());
    Assert.Equal(RefreshToken.SecretLength, Convert.FromBase64String(refreshToken.Secret).Length);
  }

  [Fact(DisplayName = "It should create a realm session.")]
  public async Task It_should_create_a_realm_session()
  {
    SetRealm();

    UserId userId = UserId.NewId(TenantId);
    ActorId actorId = new(userId.Value);
    UniqueName uniqueName = new(Realm.UniqueNameSettings, UsernameString);
    User user = new(uniqueName, actorId, userId);
    user.SetPassword(_passwordManager.ValidateAndCreate(PasswordString), actorId);
    await _userRepository.SaveAsync(user);

    SignInSessionPayload payload = new(uniqueName.Value, PasswordString);
    SignInSessionCommand command = new(payload);
    SessionModel session = await ActivityPipeline.ExecuteAsync(command);

    Assert.False(session.IsPersistent);
    Assert.Null(session.RefreshToken);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Empty(session.CustomAttributes);
    Assert.Equal(payload.UniqueName, session.User.UniqueName);
    Assert.Same(Realm, session.User.Realm);
  }

  [Fact(DisplayName = "It should create a session to an user with its email as unique name.")]
  public async Task It_should_create_a_session_to_an_user_with_its_email_as_unique_name()
  {
    SetRealm();

    User user = new(new UniqueName(Realm.UniqueNameSettings, Faker.Person.Email), id: UserId.NewId(TenantId));
    user.SetEmail(new Email(Faker.Person.Email, isVerified: true));
    user.SetPassword(_passwordManager.ValidateAndCreate(PasswordString));
    await _userRepository.SaveAsync(user);

    SignInSessionPayload payload = new(Faker.Person.Email, PasswordString);
    SignInSessionCommand command = new(payload);
    SessionModel session = await ActivityPipeline.ExecuteAsync(command);

    Assert.False(session.IsPersistent);
    Assert.Null(session.RefreshToken);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Empty(session.CustomAttributes);
    Assert.Equal(payload.UniqueName, session.User.UniqueName);
    Assert.Same(Realm, session.User.Realm);
  }

  [Fact(DisplayName = "It should create an ephemereal session.")]
  public async Task It_should_create_an_ephemereal_session()
  {
    SignInSessionPayload payload = new(UsernameString, PasswordString);
    SignInSessionCommand command = new(payload);
    SessionModel session = await ActivityPipeline.ExecuteAsync(command);

    Assert.False(session.IsPersistent);
    Assert.Null(session.RefreshToken);
    Assert.True(session.IsActive);
    Assert.Null(session.SignedOutBy);
    Assert.Null(session.SignedOutOn);
    Assert.Empty(session.CustomAttributes);
    Assert.Equal(UsernameString, session.User.UniqueName);
  }

  [Fact(DisplayName = "It should throw IncorrectUserPasswordException when the password is incorrect.")]
  public async Task It_should_throw_IncorrectUserPasswordException_when_the_password_is_incorrect()
  {
    SignInSessionPayload payload = new(UsernameString, PasswordString[..^1]);
    SignInSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<IncorrectUserPasswordException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(payload.Password, exception.AttemptedPassword);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple users are found.")]
  public async Task It_should_throw_TooManyResultsException_when_multiple_users_are_found()
  {
    User user = (await _userRepository.LoadAsync()).Single();
    user.SetEmail(new Email(Faker.Person.Email, isVerified: true));
    User other = new(new UniqueName(new ReadOnlyUniqueNameSettings(), Faker.Person.Email));
    await _userRepository.SaveAsync([user, other]);

    SignInSessionPayload payload = new(Faker.Person.Email, PasswordString);
    SignInSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<User>>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }

  [Fact(DisplayName = "It should throw UserNotFoundException when signing-in by ID.")]
  public async Task It_should_throw_UserNotFoundException_when_signing_in_by_Id()
  {
    SetRealm();

    User user = new(new UniqueName(Realm.UniqueNameSettings, UsernameString), id: UserId.NewId(TenantId));
    user.SetPassword(_passwordManager.ValidateAndCreate(PasswordString));
    await _userRepository.SaveAsync(user);

    SignInSessionPayload payload = new(user.EntityId.ToGuid().ToString(), PasswordString);
    SignInSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(TenantId.Value, exception.TenantId);
    Assert.Equal(payload.UniqueName, exception.User);
    Assert.Equal("UniqueName", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw UserNotFoundException when the user could not be found.")]
  public async Task It_should_throw_UserNotFoundException_when_the_user_could_not_be_found()
  {
    SetRealm();

    SignInSessionPayload payload = new(UsernameString, PasswordString);
    SignInSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(TenantId.Value, exception.TenantId);
    Assert.Equal(payload.UniqueName, exception.User);
    Assert.Equal("UniqueName", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    SignInSessionPayload payload = new(UsernameString, password: string.Empty);
    SignInSessionCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("Password", exception.Errors.Single().PropertyName);
  }
}
