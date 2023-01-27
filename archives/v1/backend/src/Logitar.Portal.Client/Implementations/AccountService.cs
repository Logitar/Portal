using Logitar.Portal.Core.Accounts.Payloads;
using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Core.Users.Models;
using Logitar.Portal.Core.Users.Payloads;
using Microsoft.Extensions.Options;

namespace Logitar.Portal.Client.Implementations
{
  internal class AccountService : HttpService, IAccountService
  {
    public AccountService(HttpClient client, IOptions<PortalSettings> settings) : base(client, settings)
    {
    }

    public async Task<SessionModel> AuthenticateWithGoogleAsync(string realm, AuthenticateWithGooglePayload payload, CancellationToken cancellationToken)
      => await PostAsync<SessionModel>($"/realms/{realm}/google/account/auth", payload, cancellationToken);

    public async Task<UserModel> ChangePasswordAsync(Guid sessionId, ChangePasswordPayload payload, CancellationToken cancellationToken)
      => await PostAsync<UserModel>("/account/password/change", payload, sessionId, cancellationToken);

    public async Task<UserModel> GetProfileAsync(Guid sessionId, CancellationToken cancellationToken)
      => await GetAsync<UserModel>("/account/profile", sessionId, cancellationToken);

    public async Task RecoverPasswordAsync(string realm, RecoverPasswordPayload payload, CancellationToken cancellationToken)
      => await PostAsync($"/realms/{realm}/account/password/recover", payload, cancellationToken);

    public async Task<SessionModel> RenewSessionAsync(string realm, RenewSessionPayload payload, CancellationToken cancellationToken)
      => await PostAsync<SessionModel>($"/realms/{realm}/account/renew", payload, cancellationToken);

    public async Task ResetPasswordAsync(string realm, ResetPasswordPayload payload, CancellationToken cancellationToken)
      => await PostAsync($"/realms/{realm}/account/password/reset", payload, cancellationToken);

    public async Task<UserModel> SaveProfileAsync(Guid sessionId, UpdateUserPayload payload, CancellationToken cancellationToken)
      => await PutAsync<UserModel>("/account/profile", payload, sessionId, cancellationToken);

    public async Task<SessionModel> SignInAsync(string realm, SignInPayload payload, CancellationToken cancellationToken)
      => await PostAsync<SessionModel>($"/realms/{realm}/account/sign/in", payload, cancellationToken);

    public async Task SignOutAsync(Guid sessionId, CancellationToken cancellationToken)
      => await PostAsync("/account/sign/out", sessionId, cancellationToken);
  }
}
