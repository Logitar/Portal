using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Users.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class AuthenticateUserCommandTests : IntegrationTests
{
  private readonly IUserRepository _userRepository;

  public AuthenticateUserCommandTests() : base()
  {
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should authenticate the user.")]
  public async Task It_should_authenticate_the_user()
  {
    AuthenticateUserPayload payload = new(Faker.Person.UserName, PasswordString);
    AuthenticateUserCommand command = new(payload);
    User result = await Mediator.Send(command);

    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    Assert.Equal(user.Id.ToGuid(), result.Id);
  }

  [Fact(DisplayName = "It should throw IncorrectUserPasswordException when the password is incorrect.")]
  public async Task It_should_throw_IncorrectUserPasswordException_when_the_password_is_incorrect()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    AuthenticateUserPayload payload = new(Faker.Person.UserName, PasswordString[..^1]);
    AuthenticateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<IncorrectUserPasswordException>(async () => await Mediator.Send(command));
    Assert.Equal(user.Id, exception.UserId);
    Assert.Equal(payload.Password, exception.AttemptedPassword);
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
    var exception = await Assert.ThrowsAsync<UserHasNoPasswordException>(async () => await Mediator.Send(command));
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
    var exception = await Assert.ThrowsAsync<UserIsDisabledException>(async () => await Mediator.Send(command));
    Assert.Equal(disabled.Id, exception.UserId);
  }

  [Fact(DisplayName = "It should throw UserNotFoundException when the user cannot be found.")]
  public async Task It_should_throw_UserNotFoundException_when_the_user_cannot_be_found()
  {
    SetRealm();

    AuthenticateUserPayload payload = new(Faker.Person.UserName, PasswordString);
    AuthenticateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(async () => await Mediator.Send(command));
    Assert.Equal(TenantId, exception.TenantId);
    Assert.Equal(payload.UniqueName, exception.User);
    Assert.Equal("UniqueName", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    AuthenticateUserPayload payload = new(Faker.Person.UserName, password: string.Empty);
    AuthenticateUserCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.Equal("Password", exception.Errors.Single().PropertyName);
  }
}
