using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Client.Implementations;

internal class UserService : HttpService, IUserService
{
  private const string BasePath = "/users";

  public UserService(HttpClient client, PortalSettings settings) : base(client, settings)
  {
  }

  public async Task<User> ChangePasswordAsync(Guid id, ChangePasswordInput input, CancellationToken cancellationToken)
    => await PatchAsync<User>($"{BasePath}/{id}/password/change", input, cancellationToken);

  public async Task<User> CreateAsync(CreateUserInput input, CancellationToken cancellationToken)
    => await PostAsync<User>(BasePath, input, cancellationToken);

  public async Task<User> DeleteAsync(Guid id, CancellationToken cancellationToken)
    => await DeleteAsync<User>($"{BasePath}/{id}", cancellationToken);

  public async Task<User> DisableAsync(Guid id, CancellationToken cancellationToken)
    => await PatchAsync<User>($"{BasePath}/{id}/disable", cancellationToken);

  public async Task<User> EnableAsync(Guid id, CancellationToken cancellationToken)
    => await PatchAsync<User>($"{BasePath}/{id}/enable", cancellationToken);

  public async Task<User?> GetAsync(Guid? id, string? realm, string? username,
    string? externalKey, string? externalValue, CancellationToken cancellationToken)
  {
    if (realm != null && (username != null || (externalKey != null && externalValue != null)))
    {
      throw new NotSupportedException("You may only query users by their identifier.");
    }

    return id.HasValue ? await GetAsync<User>($"{BasePath}/{id.Value}", cancellationToken) : null;
  }

  public async Task<PagedList<User>> GetAsync(bool? isConfirmed, bool? isDisabled, string? realm, string? search,
    UserSort? sort, bool isDescending, int? skip, int? limit, CancellationToken cancellationToken)
  {
    string query = GetQueryString(new Dictionary<string, object?>
    {
      [nameof(isConfirmed)] = isConfirmed,
      [nameof(isDisabled)] = isDisabled,
      [nameof(realm)] = realm,
      [nameof(search)] = search,
      [nameof(sort)] = sort,
      [nameof(isDescending)] = isDescending,
      [nameof(skip)] = skip,
      [nameof(limit)] = limit
    });

    return await GetAsync<PagedList<User>>($"{BasePath}?{query}", cancellationToken);
  }

  public async Task<SentMessages> RecoverPasswordAsync(RecoverPasswordInput input, CancellationToken cancellationToken)
    => await PostAsync<SentMessages>($"{BasePath}/password/recover", input, cancellationToken);

  public async Task<User> SetExternalIdentifierAsync(Guid id, string key, string? value, CancellationToken cancellationToken)
    => await PatchAsync<User>($"{BasePath}/{id}/external-identifiers/{key}?value={value}", cancellationToken);

  public async Task<User> UpdateAsync(Guid id, UpdateUserInput input, CancellationToken cancellationToken)
    => await PutAsync<User>($"{BasePath}/{id}", input, cancellationToken);

  public async Task<User> VerifyAddressAsync(Guid id, CancellationToken cancellationToken)
    => await PatchAsync<User>($"{BasePath}/{id}/address/verify", cancellationToken);

  public async Task<User> VerifyEmailAsync(Guid id, CancellationToken cancellationToken)
    => await PatchAsync<User>($"{BasePath}/{id}/email/verify", cancellationToken);

  public async Task<User> VerifyPhoneAsync(Guid id, CancellationToken cancellationToken)
    => await PatchAsync<User>($"{BasePath}/{id}/phone/verify", cancellationToken);
}
