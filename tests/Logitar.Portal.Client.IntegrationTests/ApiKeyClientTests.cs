using Bogus;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Roles;

namespace Logitar.Portal.Client;

internal class ApiKeyClientTests
{
  private const string Sut = "ApiKeyClient";

  private readonly TestContext _context;
  private readonly Faker _faker;
  private readonly IApiKeyService _apiKeyService;

  public ApiKeyClientTests(TestContext context, Faker faker, IApiKeyService apiKeyService)
  {
    _context = context;
    _faker = faker;
    _apiKeyService = apiKeyService;
  }

  public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
  {
    string name = string.Empty;

    try
    {
      name = $"{Sut}.{nameof(_apiKeyService.CreateAsync)}";
      CreateApiKeyPayload create = new()
      {
        Realm = _context.Realm.UniqueSlug,
        Title = "Default API Key"
      };
      ApiKey apiKey = await _apiKeyService.CreateAsync(create, cancellationToken);
      string xApiKey = apiKey.XApiKey ?? string.Empty;
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_apiKeyService.ReplaceAsync)}";
      ReplaceApiKeyPayload replace = new()
      {
        Title = apiKey.Title,
        Description = "This is the default API key.",
        ExpiresOn = DateTime.Now.AddYears(1),
        Roles = new string[] { $"  {_context.Role.UniqueName}  " }
      };
      apiKey = await _apiKeyService.ReplaceAsync(apiKey.Id, replace, apiKey.Version, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_apiKeyService.UpdateAsync)}";
      UpdateApiKeyPayload update = new()
      {
        Roles = new RoleModification[]
        {
          new(_context.Role.Id.ToString(), CollectionAction.Remove)
        }
      };
      apiKey = await _apiKeyService.UpdateAsync(apiKey.Id, update, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_apiKeyService.DeleteAsync)}";
      ApiKey delete = await _apiKeyService.CreateAsync(new CreateApiKeyPayload
      {
        Realm = _context.Realm.Id.ToString(),
        Title = apiKey.Title,
      }, cancellationToken);
      delete = await _apiKeyService.DeleteAsync(delete.Id, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_apiKeyService.SearchAsync)}";
      SearchApiKeysPayload search = new()
      {
        Realm = $"  {_context.Realm.UniqueSlug}  ",
        IdIn = new Guid[] { apiKey.Id }
      };
      SearchResults<ApiKey> results = await _apiKeyService.SearchAsync(search, cancellationToken);
      apiKey = results.Results.Single();
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_apiKeyService.ReadAsync)}:null";
      ApiKey? result = await _apiKeyService.ReadAsync(Guid.Empty, cancellationToken: cancellationToken);
      if (result != null)
      {
        throw new InvalidOperationException("The apiKey should be null.");
      }
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_apiKeyService.ReadAsync)}:Id";
      apiKey = await _apiKeyService.ReadAsync(apiKey.Id, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_apiKeyService.AuthenticateAsync)}";
      apiKey = await _apiKeyService.AuthenticateAsync(xApiKey, cancellationToken);
      _context.Succeed(name);
    }
    catch (Exception exception)
    {
      _context.Fail(name, exception);
    }

    return !_context.HasFailed;
  }
}
