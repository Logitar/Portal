using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Users.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class AuthenticateUserCommandTests : IntegrationTests
{
  private readonly IPasswordManager _passwordManager;
  private readonly IUserRepository _userRepository;

  public AuthenticateUserCommandTests() : base()
  {
    _passwordManager = ServiceProvider.GetRequiredService<IPasswordManager>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should authenticate an user with its email as unique name.")]
  public async Task It_should_authenticate_an_user_with_its_email_as_unique_name()
  {
    SetRealm();

    UserAggregate user = new(new UniqueNameUnit(Realm.UniqueNameSettings, Faker.Person.Email), TenantId);
    user.SetEmail(new EmailUnit(Faker.Person.Email, isVerified: true));
    user.SetPassword(_passwordManager.ValidateAndCreate(PasswordString));
    await _userRepository.SaveAsync(user);

    AuthenticateUserPayload payload = new(Faker.Person.Email, PasswordString);
    AuthenticateUserCommand command = new(payload);
    UserModel result = await ActivityPipeline.ExecuteAsync(command);

    Assert.Equal(user.Id.ToGuid(), result.Id);
  }

  [Fact(DisplayName = "It should authenticate the user.")]
  public async Task It_should_authenticate_the_user()
  {
    AuthenticateUserPayload payload = new(UsernameString, PasswordString);
    AuthenticateUserCommand command = new(payload);
    UserModel result = await ActivityPipeline.ExecuteAsync(command);

    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    Assert.Equal(user.Id.ToGuid(), result.Id);
  }

  [Fact(DisplayName = "It should throw IncorrectUserPasswordException when the password is incorrect.")]
  public async Task It_should_throw_IncorrectUserPasswordException_when_the_password_is_incorrect()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    AuthenticateUserPayload payload = new(UsernameString, PasswordString[..^1]);
    AuthenticateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<IncorrectUserPasswordException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(user.Id, exception.UserId);
    Assert.Equal(payload.Password, exception.AttemptedPassword);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple users are found.")]
  public async Task It_should_throw_TooManyResultsException_when_multiple_users_are_found()
  {
    UserAggregate user = (await _userRepository.LoadAsync()).Single();
    user.SetEmail(new EmailUnit(Faker.Person.Email, isVerified: true));
    UserAggregate other = new(new UniqueNameUnit(new ReadOnlyUniqueNameSettings(), Faker.Person.Email));
    await _userRepository.SaveAsync([user, other]);

    AuthenticateUserPayload payload = new(Faker.Person.Email, PasswordString);
    AuthenticateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<UserAggregate>>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }

  [Fact(DisplayName = "It should throw UserHasNoPasswordException when the user has no password.")]
  public async Task It_should_throw_UserHasNoPasswordException_when_the_user_has_no_password()
  {
    SetRealm();

    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    UserAggregate other = new(user.UniqueName, TenantId);
    await _userRepository.SaveAsync(other);

    AuthenticateUserPayload payload = new(other.UniqueName.Value, PasswordString);
    AuthenticateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UserHasNoPasswordException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(other.Id, exception.UserId);
  }

  [Fact(DisplayName = "It should throw UserIsDisabledException when the user is disabled.")]
  public async Task It_should_throw_UserIsDisabledException_when_the_user_is_disabled()
  {
    SetRealm();

    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    UserAggregate disabled = new(user.UniqueName, TenantId);
    disabled.Disable();
    await _userRepository.SaveAsync(disabled);

    AuthenticateUserPayload payload = new(disabled.UniqueName.Value, PasswordString);
    AuthenticateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UserIsDisabledException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(disabled.Id, exception.UserId);
  }

  [Fact(DisplayName = "It should throw UserNotFoundException when authenticating by ID.")]
  public async Task It_should_throw_UserNotFoundException_when_authenticating_by_Id()
  {
    SetRealm();

    UserAggregate user = new(new UniqueNameUnit(Realm.UniqueNameSettings, UsernameString), TenantId);
    user.SetPassword(_passwordManager.ValidateAndCreate(PasswordString));
    await _userRepository.SaveAsync(user);

    AuthenticateUserPayload payload = new(user.Id.ToGuid().ToString(), PasswordString);
    AuthenticateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(TenantId, exception.TenantId);
    Assert.Equal(payload.UniqueName, exception.User);
    Assert.Equal("UniqueName", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw UserNotFoundException when the user cannot be found.")]
  public async Task It_should_throw_UserNotFoundException_when_the_user_cannot_be_found()
  {
    SetRealm();

    AuthenticateUserPayload payload = new(UsernameString, PasswordString);
    AuthenticateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(TenantId, exception.TenantId);
    Assert.Equal(payload.UniqueName, exception.User);
    Assert.Equal("UniqueName", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    AuthenticateUserPayload payload = new(UsernameString, password: string.Empty);
    AuthenticateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("Password", exception.Errors.Single().PropertyName);
  }
}
