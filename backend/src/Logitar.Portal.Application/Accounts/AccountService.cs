using Logitar.Portal.Application.Accounts.Commands;
using Logitar.Portal.Application.Sessions.Commands;
using Logitar.Portal.Application.Users.Commands;
using Logitar.Portal.Application.Users.Queries;
using Logitar.Portal.Contracts.Accounts;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Accounts
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
      return await _requestPipeline.ExecuteAsync(new GetUserQuery(_userContext.UserId), cancellationToken);
    }

    public async Task RecoverPasswordAsync(RecoverPasswordPayload payload, string realm, CancellationToken cancellationToken)
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }

    public async Task<SessionModel> RenewSessionAsync(RenewSessionPayload payload, string? realm, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new RenewSessionCommand(payload, realm), cancellationToken);
    }

    public async Task ResetPasswordAsync(ResetPasswordPayload payload, string realm, CancellationToken cancellationToken)
    {
      throw new NotImplementedException(); // TODO(fpion): implement
    }

    public async Task<UserModel> SaveProfileAsync(UpdateUserPayload payload, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new UpdateUserCommand(_userContext.UserId, payload), cancellationToken);
    }

    public async Task<SessionModel> SignInAsync(SignInPayload payload, string? realm, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new SignInCommand(payload, realm), cancellationToken);
    }

    public async Task SignOutAsync(CancellationToken cancellationToken)
    {
      await _requestPipeline.ExecuteAsync(new SignOutCommand(_userContext.SessionId), cancellationToken);
    }
  }
}
