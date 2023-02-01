using Logitar.Portal.Contracts.Accounts.Payloads;
using Logitar.Portal.Contracts.Sessions.Models;

namespace Logitar.Portal.Application.Accounts
{
  public interface IAccountService
  {
    Task<SessionModel> SignInAsync(SignInPayload payload, string? realm = null, CancellationToken cancellationToken = default);
  }
}
