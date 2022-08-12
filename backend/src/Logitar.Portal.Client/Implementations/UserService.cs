using Logitar.Portal.Core;
using Logitar.Portal.Core.Users;
using Logitar.Portal.Core.Users.Models;
using Logitar.Portal.Core.Users.Payloads;
using Microsoft.Extensions.Options;

namespace Logitar.Portal.Client.Implementations
{
  internal class UserService : HttpService, IUserService
  {
    private const string BasePath = "/users";

    public UserService(HttpClient client, IOptions<PortalSettings> settings) : base(client, settings)
    {
    }

    public async Task<UserModel> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken)
      => await PostAsync<UserModel>(BasePath, payload, cancellationToken);

    public async Task<UserModel> DeleteAsync(Guid id, CancellationToken cancellationToken)
      => await DeleteAsync<UserModel>($"{BasePath}/{id}", cancellationToken);

    public async Task<UserModel> DisableAsync(Guid id, CancellationToken cancellationToken)
      => await PatchAsync<UserModel>($"{BasePath}/{id}/disable", cancellationToken);

    public async Task<UserModel> EnableAsync(Guid id, CancellationToken cancellationToken)
      => await PatchAsync<UserModel>($"{BasePath}/{id}/enable", cancellationToken);

    public async Task<UserModel> GetAsync(Guid id, CancellationToken cancellationToken)
      => await GetAsync<UserModel>($"{BasePath}/{id}", cancellationToken);

    public async Task<ListModel<UserSummary>> GetAsync(bool? isConfirmed, bool? isDisabled, string? realm, string? search, UserSort? sort, bool desc, int? index, int? count, CancellationToken cancellationToken)
    {
      string query = GetQueryString(new Dictionary<string, object?>
      {
        [nameof(isConfirmed)] = isConfirmed,
        [nameof(isDisabled)] = isDisabled,
        [nameof(realm)] = realm,
        [nameof(search)] = search,
        [nameof(sort)] = sort,
        [nameof(desc)] = desc,
        [nameof(index)] = index,
        [nameof(count)] = count
      });

      return await GetAsync<ListModel<UserSummary>>($"{BasePath}?{query}", cancellationToken);
    }

    public async Task<UserModel> UpdateAsync(Guid id, UpdateUserPayload payload, CancellationToken cancellationToken)
      => await PutAsync<UserModel>($"{BasePath}/{id}", payload, cancellationToken);
  }
}
