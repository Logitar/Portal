using Logitar.Portal.Contracts.Accounts.Payloads;
using Logitar.Portal.Contracts.Sessions.Models;
using Logitar.Portal.Contracts.Users.Models;
using Logitar.Portal.Contracts.Users.Payloads;

namespace Logitar.Portal.Application.Accounts
{
  public interface IAccountService
  {
    Task<UserModel> ChangePasswordAsync(ChangePasswordPayload payload, CancellationToken cancellationToken = default);
    Task<UserModel> GetProfileAsync(CancellationToken cancellationToken = default);
    Task RecoverPasswordAsync(RecoverPasswordPayload payload, string realm, CancellationToken cancellationToken = default);
    Task<SessionModel> RenewSessionAsync(RenewSessionPayload payload, string? realm = null, CancellationToken cancellationToken = default);
    Task ResetPasswordAsync(ResetPasswordPayload payload, string realm, CancellationToken cancellationToken = default);
    Task<UserModel> SaveProfileAsync(UpdateUserPayload payload, CancellationToken cancellationToken = default);
    Task<SessionModel> SignInAsync(SignInPayload payload, string? realm = null, CancellationToken cancellationToken = default);
    Task SignOutAsync(CancellationToken cancellationToken = default);
  }
}
