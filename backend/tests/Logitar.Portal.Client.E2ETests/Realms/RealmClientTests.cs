using Logitar.Portal.Client;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Microsoft.Extensions.Configuration;

namespace Logitar.Portal.Realms;

internal class RealmClientTests : IClientTests
{
  private readonly Random _random = new();

  private readonly IRealmClient _client;
  private readonly string _uniqueSlug;

  public RealmClientTests(IRealmClient client, IConfiguration configuration)
  {
    _client = client;

    PortalSettings settings = configuration.GetSection(PortalSettings.SectionKey).Get<PortalSettings>() ?? new();
    if (string.IsNullOrWhiteSpace(settings.Realm))
    {
      throw new ArgumentException("The configuration 'Portal.Realm' is required.", nameof(configuration));
    }
    _uniqueSlug = settings.Realm.Trim();
  }

  public async Task<bool> ExecuteAsync(TestContext context)
  {
    try
    {
      context.SetName(_client.GetType(), nameof(_client.CreateAsync));
      CreateRealmPayload create = new(_uniqueSlug, secret: string.Empty);
      RealmModel realm = await _client.CreateAsync(create, context.Request);
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.DeleteAsync));
      realm = await _client.DeleteAsync(realm.Id, context.Request)
        ?? throw new InvalidOperationException("The realm should not be null.");
      realm = await _client.CreateAsync(create, context.Request);
      context.SetRealm(realm);
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.ReadAsync));
      RealmModel? notFound = await _client.ReadAsync(Guid.NewGuid(), uniqueSlug: null, context.Request);
      if (notFound != null)
      {
        throw new InvalidOperationException("The realm should not be found.");
      }
      realm = await _client.ReadAsync(realm.Id, realm.UniqueSlug, context.Request)
        ?? throw new InvalidOperationException("The realm should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.SearchAsync));
      SearchRealmsPayload search = new();
      string[] terms = realm.UniqueSlug.Split('-');
      search.Search.Terms.Add(new SearchTerm($"%{terms[_random.Next(0, terms.Length)]}%"));
      SearchResults<RealmModel> results = await _client.SearchAsync(search, context.Request);
      realm = results.Items.Single();
      context.Succeed();

      long version = realm.Version;

      context.SetName(_client.GetType(), nameof(_client.UpdateAsync));
      UpdateRealmPayload update = new()
      {
        DisplayName = new ChangeModel<string>("Test Realm"),
        DefaultLocale = new ChangeModel<string>("en")
      };
      realm = await _client.UpdateAsync(realm.Id, update, context.Request)
        ?? throw new InvalidOperationException("The realm should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.ReplaceAsync));
      ReplaceRealmPayload replace = new(realm.UniqueSlug, realm.Secret)
      {
        DisplayName = null,
        Description = realm.Description,
        DefaultLocale = realm.DefaultLocale?.Code,
        Url = realm.Url,
        UniqueNameSettings = realm.UniqueNameSettings,
        PasswordSettings = realm.PasswordSettings,
        RequireUniqueEmail = realm.RequireUniqueEmail,
        CustomAttributes = realm.CustomAttributes
      };
      realm = await _client.ReplaceAsync(realm.Id, replace, version, context.Request)
        ?? throw new InvalidOperationException("The realm should not be null.");
      context.Succeed();
    }
    catch (Exception exception)
    {
      context.Fail(exception);
    }

    return !context.HasFailed;
  }
}
