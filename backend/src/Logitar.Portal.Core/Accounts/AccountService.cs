using Logitar.Portal.Core.Accounts.Commands;
using Logitar.Portal.Core.Accounts.Payloads;
using Logitar.Portal.Core.Sessions.Commands;
using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Core.Users.Commands;
using Logitar.Portal.Core.Users.Models;
using Logitar.Portal.Core.Users.Payloads;
using Logitar.Portal.Core.Users.Queries;

namespace Logitar.Portal.Core.Accounts
{
  internal class AccountService : IAccountService
  {
    private readonly IRequestPipeline _requestPipeline;
    private readonly IUserContext _userContext;

    public AccountService(IRequestPipeline requestPipeline, IUserContext userContext)
    {
      _requestPipeline = requestPipeline;
      _userContext = userContext;
    }

    public async Task<UserModel> ChangePasswordAsync(ChangePasswordPayload payload, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new ChangePasswordCommand(payload), cancellationToken);
    }

    public async Task<UserModel> GetProfileAsync(CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new GetUserQuery(_userContext.Id), cancellationToken)
        ?? throw new InvalidOperationException($"The user 'Id={_userContext.Id}' could not be found.");
    }

    public async Task RecoverPasswordAsync(RecoverPasswordPayload payload, string realm, CancellationToken cancellationToken)
    {
      await _requestPipeline.ExecuteAsync(new RecoverPasswordCommand(payload, realm), cancellationToken);
    }

    public async Task<SessionModel> RenewSessionAsync(RenewSessionPayload payload, string? realm, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new RenewSessionCommand(payload, realm), cancellationToken);
    }

    public async Task ResetPasswordAsync(ResetPasswordPayload payload, string realm, CancellationToken cancellationToken)
    {
      await _requestPipeline.ExecuteAsync(new ResetPasswordCommand(payload, realm), cancellationToken);
    }

    public async Task<UserModel> SaveProfileAsync(UpdateUserPayload payload, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new UpdateUserCommand(_userContext.Id, payload), cancellationToken);
    }

    public async Task<SessionModel> SignInAsync(SignInPayload payload, string? realm, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new SignInCommand(payload, realm), cancellationToken);
    }

    public async Task SignOutAsync(CancellationToken cancellationToken)
    {
      await _requestPipeline.ExecuteAsync(new SignOutSessionCommand(_userContext.SessionId), cancellationToken);
    }
  }
}
