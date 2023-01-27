using Logitar.Portal.Core2.Accounts.Commands;
using Logitar.Portal.Core2.Accounts.Payloads;
using Logitar.Portal.Core2.Sessions.Models;

namespace Logitar.Portal.Core2.Accounts
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
