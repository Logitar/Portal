using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Client;

internal class UserClient : ClientBase, IUserService
{
  private const string Path = "/api/users";

  public UserClient(HttpClient client, PortalSettings settings) : base(client, settings)
  {
  }

  public async Task<User> AuthenticateAsync(AuthenticateUserPayload payload, CancellationToken cancellationToken)
  {
    return await PatchAsync<User>($"{Path}/authenticate", payload, cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
  }

  public async Task<User> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<User>(Path, payload, cancellationToken)
      ?? throw new InvalidOperationException("The result should not be null.");
  }

  public async Task<User?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await DeleteAsync<User>($"{Path}/{id}", cancellationToken);
  }

  public async Task<User?> ReadAsync(Guid? id, string? realm, string? uniqueName,
    string? identifierKey, string? identifierValue, CancellationToken cancellationToken)
  {
    Dictionary<Guid, User> users = new(capacity: 2);

    if (id.HasValue)
    {
      User? user = await GetAsync<User>($"{Path}/{id}", cancellationToken);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (uniqueName != null)
    {
      StringBuilder path = new();

      path.Append(Path).Append("/unique-name:").Append(uniqueName);
      if (realm != null)
      {
        path.Append("?realm=").Append(realm);
      }

      User? user = await GetAsync<User>(path.ToString(), cancellationToken);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (identifierKey != null && identifierValue != null)
    {
      StringBuilder path = new();

      path.Append(Path).Append("/identifiers/key:").Append(identifierKey).Append("/value:").Append(identifierValue);
      if (realm != null)
      {
        path.Append("?realm=").Append(realm);
      }

      User? user = await GetAsync<User>(path.ToString(), cancellationToken);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (users.Count > 1)
    {
      throw new TooManyResultsException<User>(expected: 1, actual: users.Count);
    }

    return users.Values.SingleOrDefault();
  }

  public async Task<User?> RemoveIdentifierAsync(Guid id, string key, CancellationToken cancellationToken)
  {
    return await DeleteAsync<User>($"{Path}/{id}/identifiers/key:{key}", cancellationToken);
  }

  public async Task<User?> ReplaceAsync(Guid id, ReplaceUserPayload payload, long? version, CancellationToken cancellationToken)
  {
    StringBuilder path = new();

    path.Append(Path).Append('/').Append(id);

    if (version.HasValue)
    {
      path.Append("?version=").Append(version.Value);
    }

    return await PutAsync<User>(path.ToString(), payload, cancellationToken);
  }

  public async Task<User?> SaveIdentifierAsync(Guid id, SaveIdentifierPayload payload, CancellationToken cancellationToken)
  {
    return await PutAsync<User>($"{Path}/{id}/identifiers", payload, cancellationToken);
  }

  public async Task<SearchResults<User>> SearchAsync(SearchUsersPayload payload, CancellationToken cancellationToken)
  {
    return await PostAsync<SearchResults<User>>($"{Path}/search", payload, cancellationToken)
      ?? throw new InvalidOperationException("The results should not be null.");
  }

  public async Task<User?> SignOutAsync(Guid id, CancellationToken cancellationToken)
  {
    return await PatchAsync<User>($"{Path}/{id}/sign/out", cancellationToken);
  }

  public async Task<User?> UpdateAsync(Guid id, UpdateUserPayload payload, CancellationToken cancellationToken)
  {
    return await PatchAsync<User>($"{Path}/{id}", payload, cancellationToken);
  }
}
