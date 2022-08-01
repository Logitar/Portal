using Portal.Core.Accounts.Payloads;
using Portal.Core.Sessions.Models;
using Portal.Core.Users.Models;
using Portal.Core.Users.Payloads;

namespace Portal.Core.Accounts
{
  public interface IAccountService
  {
    Task<UserModel> ChangePasswordAsync(ChangePasswordPayload payload, CancellationToken cancellationToken = default);
    Task<UserModel> GetProfileAsync(CancellationToken cancellationToken = default);
    Task RecoverPasswordAsync(RecoverPasswordPayload payload, string realm, CancellationToken cancellationToken = default);
    Task<SessionModel> RenewSessionAsync(RenewSessionPayload payload, string? realm = null, string? ipAddress = null, string? additionalInformation = null, CancellationToken cancellationToken = default);
    Task ResetPasswordAsync(ResetPasswordPayload payload, string realm, CancellationToken cancellationToken = default);
    Task<UserModel> SaveProfileAsync(UpdateUserPayload payload, CancellationToken cancellationToken = default);
    Task<SessionModel> SignInAsync(SignInPayload payload, string? realm = null, string? ipAddress = null, string? additionalInformation = null, CancellationToken cancellationToken = default);
    Task SignOutAsync(CancellationToken cancellationToken = default);
  }
}
