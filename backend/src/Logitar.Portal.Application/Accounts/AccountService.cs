using Logitar.Portal.Application.Accounts.Commands;
using Logitar.Portal.Contracts.Accounts.Payloads;
using Logitar.Portal.Contracts.Sessions.Models;

namespace Logitar.Portal.Application.Accounts
{
  internal class AccountService : IAccountService
  {
    private readonly IRequestPipeline _requestPipeline;

    public AccountService(IRequestPipeline requestPipeline)
    {
      _requestPipeline = requestPipeline;
    }

    public async Task<SessionModel> SignInAsync(SignInPayload payload, string? realm, CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new SignInCommand(payload, realm), cancellationToken);
    }
  }
}
