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

  public async Task<User> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new DeleteUser(id), cancellationToken);
  }

  public async Task<User> DisableAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new DisableUser(id), cancellationToken);
  }

  public async Task<User> EnableAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new EnableUser(id), cancellationToken);
  }

  public async Task<User> SetExternalIdentifierAsync(Guid id, string key, string? value, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new SetExternalIdentifier(id, key, value), cancellationToken);
  }

  public async Task<User> UpdateAsync(Guid id, UpdateUserInput input, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new UpdateUser(id, input), cancellationToken);
  }

  public async Task<User> VerifyAddressAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new VerifyAddress(id), cancellationToken);
  }

  public async Task<User> VerifyEmailAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new VerifyEmail(id), cancellationToken);
  }

  public async Task<User> VerifyPhoneAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new VerifyPhone(id), cancellationToken);
  }
}
