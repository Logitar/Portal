using Logitar.Portal.Core.Accounts.Commands;
using Logitar.Portal.Core.Accounts.Payloads;
using Logitar.Portal.Core.Sessions.Models;

namespace Logitar.Portal.Core.Accounts
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
