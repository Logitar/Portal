using Logitar.Net.Http;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Client.Users;

internal class UserClient : BaseClient, IUserClient
{
  private const string Path = "/api/users";
  private static Uri UriPath => new(Path, UriKind.Relative);

  public UserClient(HttpClient client, IPortalSettings settings) : base(client, settings)
  {
  }

  public async Task<UserModel> AuthenticateAsync(AuthenticateUserPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/authenticate", UriKind.Relative);
    return await PatchAsync<UserModel>(uri, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(AuthenticateAsync), HttpMethod.Patch, uri, payload, context);
  }

  public async Task<UserModel> CreateAsync(CreateUserPayload payload, IRequestContext? context)
  {
    return await PostAsync<UserModel>(UriPath, payload, context)
      ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, UriPath, payload, context);
  }

  public async Task<UserModel?> DeleteAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await DeleteAsync<UserModel>(uri, context);
  }

  public async Task<UserModel?> ReadAsync(Guid? id, string? uniqueName, CustomIdentifier? identifier, IRequestContext? context)
  {
    Dictionary<Guid, UserModel> users = new(capacity: 2);

    if (id.HasValue)
    {
      Uri uri = new($"{Path}/{id}", UriKind.Relative);
      UserModel? user = await GetAsync<UserModel>(uri, context);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (!string.IsNullOrWhiteSpace(uniqueName))
    {
      Uri uri = new($"{Path}/unique-name:{uniqueName}", UriKind.Relative);
      UserModel? user = await GetAsync<UserModel>(uri, context);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (identifier != null)
    {
      Uri uri = new($"{Path}/identifier/key:{identifier.Key}/value:{identifier.Value}", UriKind.Relative);
      UserModel? user = await GetAsync<UserModel>(uri, context);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (users.Count > 1)
    {
      throw TooManyResultsException<UserModel>.ExpectedSingle(users.Count);
    }

    return users.Values.SingleOrDefault();
  }

  public async Task<UserModel?> RemoveIdentifierAsync(Guid id, string key, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}/identifiers/key:{key}", UriKind.Relative);
    return await DeleteAsync<UserModel>(uri, context);
  }

  public async Task<UserModel?> ReplaceAsync(Guid id, ReplaceUserPayload payload, long? version, IRequestContext? context)
  {
    Uri uri = new UrlBuilder().SetPath($"{Path}/{id}").SetVersion(version).BuildUri(UriKind.Relative);
    return await PutAsync<UserModel>(uri, payload, context);
  }

  public async Task<UserModel?> ResetPasswordAsync(Guid id, ResetUserPasswordPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}/password/reset", UriKind.Relative);
    return await PatchAsync<UserModel>(uri, payload, context);
  }

  public async Task<UserModel?> SaveIdentifierAsync(Guid id, string key, SaveUserIdentifierPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}/identifiers/key:{key}", UriKind.Relative);
    return await PutAsync<UserModel>(uri, payload, context);
  }

  public async Task<SearchResults<UserModel>> SearchAsync(SearchUsersPayload payload, IRequestContext? context)
  {
    IUrlBuilder builder = new UrlBuilder().SetPath(Path).SetQuery(payload);
    if (payload.HasAuthenticated.HasValue)
    {
      builder.SetQuery("has_authenticated", payload.HasAuthenticated.Value.ToString());
    }
    if (payload.HasPassword.HasValue)
    {
      builder.SetQuery("has_password", payload.HasPassword.Value.ToString());
    }
    if (payload.IsDisabled.HasValue)
    {
      builder.SetQuery("disabled", payload.IsDisabled.Value.ToString());
    }
    if (payload.IsConfirmed.HasValue)
    {
      builder.SetQuery("confirmed", payload.IsConfirmed.Value.ToString());
    }
    Uri uri = builder.BuildUri(UriKind.Relative);

    return await GetAsync<SearchResults<UserModel>>(uri, context)
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri, payload, context);
  }

  public async Task<UserModel?> SignOutAsync(Guid id, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}/sign/out", UriKind.Relative);
    return await PutAsync<UserModel>(uri, content: null, context);
  }

  public async Task<UserModel?> UpdateAsync(Guid id, UpdateUserPayload payload, IRequestContext? context)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return await PatchAsync<UserModel>(uri, payload, context);
  }
}
