using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Client.Implementations;

internal class SessionService : HttpService, ISessionService
{
  private const string BasePath = "/sessions";

  public SessionService(HttpClient client, PortalSettings settings) : base(client, settings)
  {
  }

  public async Task<Session?> GetAsync(Guid? id, CancellationToken cancellationToken)
    => id.HasValue ? await GetAsync<Session>($"{BasePath}/{id.Value}", cancellationToken) : null;

  public async Task<PagedList<Session>> GetAsync(bool? isActive, bool? isPersistent, string? realm, Guid? userId,
    SessionSort? sort, bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    string query = GetQueryString(new Dictionary<string, object?>
    {
      [nameof(isActive)] = isActive,
      [nameof(isPersistent)] = isPersistent,
      [nameof(realm)] = realm,
      [nameof(userId)] = userId,
      [nameof(sort)] = sort,
      [nameof(isDescending)] = isDescending,
      [nameof(skip)] = skip,
      [nameof(limit)] = limit
    });

    return await GetAsync<PagedList<Session>>($"{BasePath}?{query}", cancellationToken);
  }

  public async Task<Session> RefreshAsync(RefreshInput input, CancellationToken cancellationToken)
    => await PostAsync<Session>($"{BasePath}/sign/in", input, cancellationToken);

  public async Task<Session> SignInAsync(SignInInput input, CancellationToken cancellationToken)
    => await PostAsync<Session>($"{BasePath}/refresh", input, cancellationToken);

  public async Task<IEnumerable<Session>> SignOutAllAsync(Guid userId, CancellationToken cancellationToken)
    => await PatchAsync<IEnumerable<Session>>($"users/{userId}/sessions/sign/out", cancellationToken);

  public async Task<Session> SignOutAsync(Guid id, CancellationToken cancellationToken)
    => await PatchAsync<Session>($"{BasePath}/{id}/sign/out", cancellationToken);

  public async Task<IEnumerable<Session>> SignOutUserAsync(Guid id, CancellationToken cancellationToken)
    => await PatchAsync<IEnumerable<Session>>($"sign/out/user/{id}", cancellationToken);
}
