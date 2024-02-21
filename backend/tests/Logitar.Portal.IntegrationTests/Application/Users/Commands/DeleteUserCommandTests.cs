using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Users.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class DeleteUserCommandTests : IntegrationTests
{
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public DeleteUserCommandTests() : base()
  {
    _sessionRepository = ServiceProvider.GetRequiredService<ISessionRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should delete an existing user.")]
  public async Task It_should_delete_an_existing_user()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    DeleteUserCommand command = new(user.Id.ToGuid());
    User? deleted = await Mediator.Send(command);
    Assert.NotNull(deleted);
    Assert.Equal(command.Id, deleted.Id);

    Assert.Empty(await _sessionRepository.LoadAsync());
  }

  [Fact(DisplayName = "It should return null when the user cannot be found.")]
  public async Task It_should_return_null_when_the_user_cannot_be_found()
  {
    DeleteUserCommand command = new(Guid.NewGuid());
    User? user = await Mediator.Send(command);
    Assert.Null(user);
  }

  [Fact(DisplayName = "It should return null when the user is in another tenant.")]
  public async Task It_should_return_null_when_the_user_is_in_another_tenant()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());

    SetRealm();

    DeleteUserCommand command = new(user.Id.ToGuid());
    User? result = await Mediator.Send(command);
    Assert.Null(result);
  }
}
