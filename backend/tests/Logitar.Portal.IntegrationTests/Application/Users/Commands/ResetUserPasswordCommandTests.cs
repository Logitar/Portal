using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Users.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ResetUserPasswordCommandTests : IntegrationTests
{
  private readonly IUserRepository _userRepository;

  public ResetUserPasswordCommandTests() : base()
  {
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should reset the user's password.")]
  public async Task It_should_reset_the_user_s_password()
  {
    const string newPassword = "Test123!";

    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    ResetUserPasswordPayload payload = new(newPassword);
    ResetUserPasswordCommand command = new(user.Id.ToGuid(), payload);
    User? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(result);
    Assert.Equal(command.Id, result.Id);
  }

  [Fact(DisplayName = "It should return null when the user cannot be found.")]
  public async Task It_should_return_null_when_the_user_cannot_be_found()
  {
    ResetUserPasswordPayload payload = new(PasswordString);
    ResetUserPasswordCommand command = new(Guid.NewGuid(), payload);
    User? user = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(user);
  }

  [Fact(DisplayName = "It should return null when the user is not in the realm.")]
  public async Task It_should_return_null_when_the_user_is_not_in_the_realm()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    SetRealm();

    ResetUserPasswordPayload payload = new(PasswordString);
    ResetUserPasswordCommand command = new(user.Id.ToGuid(), payload);
    User? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ResetUserPasswordPayload payload = new(password: string.Empty);
    ResetUserPasswordCommand command = new(Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.All(exception.Errors, e => Assert.Equal("Password", e.PropertyName));
  }
}
