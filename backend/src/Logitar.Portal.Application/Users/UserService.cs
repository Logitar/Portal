using Logitar.Portal.Application.Users.Commands;
using Logitar.Portal.Application.Users.Queries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users
{
  internal class UserService : IUserService
  {
    private readonly IRequestPipeline _requestPipeline;

    public UserService(IRequestPipeline requestPipeline)
    {
      _requestPipeline = requestPipeline;
    }

    public async Task<UserModel> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new CreateUserCommand(payload), cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
      await _requestPipeline.ExecuteAsync(new DeleteUserCommand(id), cancellationToken);
    }

    public async Task<UserModel> DisableAsync(string id, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new DisableUserCommand(id), cancellationToken);
    }

    public async Task<UserModel> EnableAsync(string id, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new EnableUserCommand(id), cancellationToken);
    }

    public async Task<UserModel?> GetAsync(string id, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new GetUserQuery(id), cancellationToken);
    }

    public async Task<ListModel<UserModel>> GetAsync(bool? isConfirmed, bool? isDisabled, string? realm, string? search,
      UserSort? sort, bool isDescending,
      int? index, int? count,
      CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new GetUsersQuery(isConfirmed, isDisabled, realm, search,
        sort, isDescending,
        index, count), cancellationToken);
    }

    public async Task<UserModel> UpdateAsync(string id, UpdateUserPayload payload, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new UpdateUserCommand(id, payload), cancellationToken);
    }
  }
}
