using Logitar.Portal.Core;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Sessions.Models;
using Microsoft.Extensions.Options;

namespace Logitar.Portal.Client.Implementations
{
  internal class SessionService : HttpService, ISessionService
  {
    private const string BasePath = "/sessions";

    public SessionService(HttpClient client, IOptions<PortalSettings> settings) : base(client, settings)
    {
    }

    public async Task<SessionModel> GetAsync(Guid id, CancellationToken cancellationToken)
      => await GetAsync<SessionModel>($"{BasePath}/{id}", cancellationToken);

    public async Task<ListModel<SessionSummary>> GetAsync(bool? isActive, bool? isPersistent, string? realm, Guid? userId, SessionSort? sort, bool desc, int? index, int? count, CancellationToken cancellationToken)
    {
      string query = GetQueryString(new Dictionary<string, object?>
      {
        [nameof(isActive)] = isActive,
        [nameof(isPersistent)] = isPersistent,
        [nameof(realm)] = realm,
        [nameof(userId)] = userId,
        [nameof(sort)] = sort,
        [nameof(desc)] = desc,
        [nameof(index)] = index,
        [nameof(count)] = count
      });

      return await GetAsync<ListModel<SessionSummary>>($"{BasePath}?{query}", cancellationToken);
    }

    public async Task<IEnumerable<SessionModel>> SignOutAllAsync(Guid userId, CancellationToken cancellationToken)
      => await PatchAsync<IEnumerable<SessionModel>>($"users/{userId}/sessions/sign/out", cancellationToken);

    public async Task<SessionModel> SignOutAsync(Guid id, CancellationToken cancellationToken)
      => await PatchAsync<SessionModel>($"{BasePath}/{id}/sign/out", cancellationToken);
  }
}
