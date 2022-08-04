using Logitar.Portal.Core;
using Logitar.Portal.Core.Accounts.Payloads;
using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Core.Users.Models;
using Logitar.Portal.Core.Users.Payloads;
using System.Text.Json;

namespace Logitar.Portal.Client
{
  internal class AccountClient : IAccountClient
  {
    private readonly HttpClient _client;

    public AccountClient(HttpClient client, PortalSettings settings)
    {
      _client = client ?? throw new ArgumentNullException(nameof(client));
      _client.BaseAddress = new Uri(settings.BaseUrl);
      _client.DefaultRequestHeaders.Add("X-API-Key", settings.ApiKey);
    }

    public async Task<UserModel> ChangePasswordAsync(Guid sessionId, ChangePasswordPayload payload, CancellationToken cancellationToken = default)
    {

    }

    public async Task<UserModel> GetProfileAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {

    }

    public async Task RecoverPasswordAsync(string realm, RecoverPasswordPayload payload, CancellationToken cancellationToken = default)
    {

    }

    public async Task<SessionModel> RenewSessionAsync(string realm, RenewSessionPayload payload, CancellationToken cancellationToken = default)
    {

    }

    public async Task ResetPasswordAsync(string realm, ResetPasswordPayload payload, CancellationToken cancellationToken = default)
    {

    }

    public async Task<UserModel> SaveProfileAsync(Guid sessionId, UpdateUserPayload payload, CancellationToken cancellationToken = default)
    {

    }

    public async Task<SessionModel> SignInAsync(string realm, SignInPayload payload, CancellationToken cancellationToken = default)
    {
      ArgumentNullException.ThrowIfNull(realm);
      ArgumentNullException.ThrowIfNull(payload);

      var requestUri = new Uri($"/api/realms/{realm}/account/sign/in", UriKind.Relative);
      using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
      {
        Content = new JsonContent(payload)
      };

      using HttpResponseMessage response = await _client.SendAsync(request, cancellationToken);

      return await HandleAsync<SessionModel>(response, cancellationToken);
    }

    public async Task SignOutAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
      var requestUri = new Uri($"/api/account/sign/out", UriKind.Relative);
      using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
      request.Headers.Add("X-Session", sessionId.ToString());

      using HttpResponseMessage response = await _client.SendAsync(request, cancellationToken);

      await HandleAsync(response, cancellationToken);
    }

    private async Task<T> HandleAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
      IReadOnlyDictionary<string, string?> data = await response.GetDataAsync(cancellationToken);
      try
      {
        response.EnsureSuccessStatusCode();
      }
      catch (Exception innerException)
      {
        var error = new Error(response.StatusCode.ToString(), description: null, data);
        throw new ErrorException(error, innerException);
      }

      // TODO(fpion): implement
    }
  }
}
