using Logitar.Portal.Application.Users.Commands;
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

  public async Task<User> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new CreateUserCommand(payload), cancellationToken);
  }

  public async Task<User?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new DeleteUserCommand(id), cancellationToken);
  }

  public Task<User?> ReadAsync(Guid? id, string? realm, string? uniqueName, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task<User?> ReplaceAsync(Guid id, ReplaceUserPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task<SearchResults<User>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task<User?> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  public Task<User?> UpdateAsync(Guid id, UpdateUserPayload payload, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
