using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Client;

internal class SessionClient : ClientBase, ISessionService
{
  private const string Path = "/api/sessions";

  public SessionClient(HttpClient client, PortalSettings settings) : base(client, settings)
  {
  }

  public async Task<Session> CreateAsync(CreateSessionPayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<Session>(Path, payload, cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
  }

  public async Task<Session?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await GetAsync<Session>($"{Path}/{id}", cancellationToken);
  }

  public async Task<Session> RenewAsync(RenewPayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<Session>($"{Path}/renew", payload, cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
  }

  public async Task<SearchResults<Session>> SearchAsync(SearchSessionsPayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<SearchResults<Session>>($"{Path}/search", payload, cancellationToken)
      ?? throw new InvalidOperationException("The results should not be null.");
  }

  public async Task<Session> SignInAsync(SignInPayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<Session>($"{Path}/sign/in", payload, cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
  }

  public async Task<Session?> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    return await PatchAsync<Session>($"{Path}/{id}/sign/out", cancellationToken);
  }
}
