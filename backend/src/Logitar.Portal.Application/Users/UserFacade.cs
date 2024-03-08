using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Users.Commands;
using Logitar.Portal.Application.Users.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users;

internal class UserFacade : IUserService
{
  private readonly IActivityPipeline _activityPipeline;

  public UserFacade(IActivityPipeline activityPipeline)
  {
    _activityPipeline = activityPipeline;
  }

  public async Task<User> AuthenticateAsync(AuthenticateUserPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new AuthenticateUserCommand(payload), cancellationToken);
  }

  public async Task<User> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new CreateUserCommand(payload), cancellationToken);
  }

  public async Task<User?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new DeleteUserCommand(id), cancellationToken);
  }

  public async Task<User?> ReadAsync(Guid? id, string? uniqueName, CustomIdentifier? identifier, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReadUserQuery(id, uniqueName, identifier), cancellationToken);
  }

  public async Task<User?> RemoveIdentifierAsync(Guid id, string key, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new RemoveUserIdentifierCommand(id, key), cancellationToken);
  }

  public async Task<User?> ReplaceAsync(Guid id, ReplaceUserPayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReplaceUserCommand(id, payload, version), cancellationToken);
  }

  public async Task<User?> ResetPasswordAsync(Guid id, ResetUserPasswordPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ResetUserPasswordCommand(id, payload), cancellationToken);
  }

  public async Task<User?> SaveIdentifierAsync(Guid id, string key, SaveUserIdentifierPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new SaveUserIdentifierCommand(id, key, payload), cancellationToken);
  }

  public async Task<SearchResults<User>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new SearchUsersQuery(payload), cancellationToken);
  }

  public async Task<User?> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new SignOutUserCommand(id), cancellationToken);
  }

  public async Task<User?> UpdateAsync(Guid id, UpdateUserPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new UpdateUserCommand(id, payload), cancellationToken);
  }
}
