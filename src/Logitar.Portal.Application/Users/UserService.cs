using Logitar.Portal.Application.Users.Commands;
using Logitar.Portal.Application.Users.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users;

internal class UserService : IUserService
{
  private readonly IRequestPipeline _pipeline;

  public UserService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<User> AuthenticateAsync(AuthenticateUserPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new AuthenticateUserCommand(payload), cancellationToken);
  }

  public async Task<User> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new CreateUserCommand(payload), cancellationToken);
  }

  public async Task<User?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new DeleteUserCommand(id), cancellationToken);
  }

  public async Task<User?> ReadAsync(Guid? id, string? realm, string? uniqueName, string? identifierKey, string? identifierValue, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ReadUserQuery(id, realm, uniqueName, identifierKey, identifierValue), cancellationToken);
  }

  public async Task<User?> RemoveIdentifierAsync(Guid id, string key, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new RemoveUserIdentifierCommand(id, key), cancellationToken);
  }

  public async Task<User?> ReplaceAsync(Guid id, ReplaceUserPayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ReplaceUserCommand(id, payload, version), cancellationToken);
  }

  public async Task<User?> SaveIdentifierAsync(Guid id, SaveIdentifierPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SaveUserIdentifierCommand(id, payload), cancellationToken);
  }

  public async Task<SearchResults<User>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SearchUsersQuery(payload), cancellationToken);
  }

  public async Task<User?> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SignOutUserCommand(id), cancellationToken);
  }

  public async Task<User?> UpdateAsync(Guid id, UpdateUserPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new UpdateUserCommand(id, payload), cancellationToken);
  }
}
