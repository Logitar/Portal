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

  public async Task<UserModel> AuthenticateAsync(AuthenticateUserPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new AuthenticateUserCommand(payload), cancellationToken);
  }

  public async Task<UserModel> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new CreateUserCommand(payload), cancellationToken);
  }

  public async Task<UserModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new DeleteUserCommand(id), cancellationToken);
  }

  public async Task<UserModel?> ReadAsync(Guid? id, string? uniqueName, CustomIdentifierModel? identifier, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReadUserQuery(id, uniqueName, identifier), cancellationToken);
  }

  public async Task<UserModel?> RemoveIdentifierAsync(Guid id, string key, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new RemoveUserIdentifierCommand(id, key), cancellationToken);
  }

  public async Task<UserModel?> ReplaceAsync(Guid id, ReplaceUserPayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReplaceUserCommand(id, payload, version), cancellationToken);
  }

  public async Task<UserModel?> ResetPasswordAsync(Guid id, ResetUserPasswordPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ResetUserPasswordCommand(id, payload), cancellationToken);
  }

  public async Task<UserModel?> SaveIdentifierAsync(Guid id, string key, SaveUserIdentifierPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new SaveUserIdentifierCommand(id, key, payload), cancellationToken);
  }

  public async Task<SearchResults<UserModel>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new SearchUsersQuery(payload), cancellationToken);
  }

  public async Task<UserModel?> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new SignOutUserCommand(id), cancellationToken);
  }

  public async Task<UserModel?> UpdateAsync(Guid id, UpdateUserPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new UpdateUserCommand(id, payload), cancellationToken);
  }
}
