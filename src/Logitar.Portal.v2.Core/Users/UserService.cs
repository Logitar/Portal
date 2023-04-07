using Logitar.Portal.v2.Contracts.Users;
using Logitar.Portal.v2.Core.Users.Commands;

namespace Logitar.Portal.v2.Core.Users;

internal class UserService : IUserService
{
  private readonly IRequestPipeline _pipeline;

  public UserService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<User> ChangePasswordAsync(Guid id, ChangePasswordInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ChangePassword(id, input), cancellationToken);
  }

  public async Task<User> CreateAsync(CreateUserInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new CreateUser(input), cancellationToken);
  }
}
