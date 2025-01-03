﻿using Logitar.Identity.Core.Sessions;
using Logitar.Identity.Core.Users;
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
    User user = Assert.Single(await _userRepository.LoadAsync());

    DeleteUserCommand command = new(user.EntityId.ToGuid());
    UserModel? deleted = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(deleted);
    Assert.Equal(command.Id, deleted.Id);

    Assert.Empty(await _sessionRepository.LoadAsync());
  }

  [Fact(DisplayName = "It should return null when the user cannot be found.")]
  public async Task It_should_return_null_when_the_user_cannot_be_found()
  {
    DeleteUserCommand command = new(Guid.NewGuid());
    UserModel? user = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(user);
  }

  [Fact(DisplayName = "It should return null when the user is in another tenant.")]
  public async Task It_should_return_null_when_the_user_is_in_another_tenant()
  {
    User user = Assert.Single(await _userRepository.LoadAsync());

    SetRealm();

    DeleteUserCommand command = new(user.EntityId.ToGuid());
    UserModel? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(result);
  }
}
