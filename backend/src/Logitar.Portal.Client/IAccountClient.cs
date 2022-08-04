using Logitar.Portal.Core.Accounts.Payloads;
using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Core.Users.Models;
using Logitar.Portal.Core.Users.Payloads;

namespace Logitar.Portal.Client
{
  public interface IAccountClient
  {
    Task<UserModel> ChangePasswordAsync(Guid sessionId, ChangePasswordPayload payload, CancellationToken cancellationToken = default);
    Task<UserModel> GetProfileAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task RecoverPasswordAsync(string realm, RecoverPasswordPayload payload, CancellationToken cancellationToken = default);
    Task ResetPasswordAsync(string realm, ResetPasswordPayload payload, CancellationToken cancellationToken = default);
    Task<SessionModel> RenewSessionAsync(string realm, RenewSessionPayload payload, CancellationToken cancellationToken = default);
    Task<UserModel> SaveProfileAsync(Guid sessionId, UpdateUserPayload payload, CancellationToken cancellationToken = default);
    Task<SessionModel> SignInAsync(string realm, SignInPayload payload, CancellationToken cancellationToken = default);
    Task SignOutAsync(Guid sessionId, CancellationToken cancellationToken = default);
  }
}
