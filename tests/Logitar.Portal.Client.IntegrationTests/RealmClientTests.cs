using Bogus;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Client;

internal class RealmClientTests
{
  private const string Sut = "RealmClient";

  private readonly TestContext _context;
  private readonly Faker _faker;
  private readonly IRealmService _realmService;

  public RealmClientTests(TestContext context, Faker faker, IRealmService realmService)
  {
    _context = context;
    _faker = faker;
    _realmService = realmService;
  }

  public async Task<bool> DeleteAsync(CancellationToken cancellationToken)
  {
    string name = $"{Sut}.{nameof(_realmService.DeleteAsync)}";
    try
    {
      Realm realm = await _realmService.DeleteAsync(_context.Realm.Id, cancellationToken)
        ?? throw new InvalidOperationException("The realm should not be null.");

      _context.Succeed(name);
    }
    catch (Exception exception)
    {
      _context.Fail(name, exception);
    }

    return !_context.HasFailed;
  }

  public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
  {
    string name = string.Empty;

    try
    {
      name = $"{Sut}.{nameof(_realmService.CreateAsync)}";
      CreateRealmPayload create = new()
      {
        UniqueSlug = "desjardins"
      };
      Realm realm = await _realmService.CreateAsync(create, cancellationToken);
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_realmService.ReplaceAsync)}";
      ReplaceRealmPayload replace = new()
      {
        UniqueSlug = realm.UniqueSlug,
        DisplayName = "Desjardins",
        DefaultLocale = _faker.Locale,
        Url = "https://www.desjardins.com/",
        RequireUniqueEmail = true,
        RequireConfirmedAccount = true,
        UniqueNameSettings = realm.UniqueNameSettings,
        PasswordSettings = realm.PasswordSettings
      };
      realm = await _realmService.ReplaceAsync(realm.Id, replace, realm.Version, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_realmService.UpdateAsync)}";
      UpdateRealmPayload update = new()
      {
        ClaimMappings = new ClaimMappingModification[]
        {
          new("Department", "department_name"),
          new("EmployeeId", "employee_no", "number")
        },
        CustomAttributes = new CustomAttributeModification[]
        {
          new("PhoneNumber", "+15148454636"),
          new("PostalAddress", "150 Saint-Catherine St W, Montreal, Quebec H2X 3Y2")
        }
      };
      realm = await _realmService.UpdateAsync(realm.Id, update, cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_realmService.SearchAsync)}";
      SearchRealmsPayload search = new()
      {
        IdIn = new Guid[] { realm.Id }
      };
      SearchResults<Realm> results = await _realmService.SearchAsync(search, cancellationToken);
      realm = results.Results.Single();
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_realmService.ReadAsync)}:null";
      Realm? result = await _realmService.ReadAsync(Guid.Empty, cancellationToken: cancellationToken);
      if (result != null)
      {
        throw new InvalidOperationException("The realm should be null.");
      }
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_realmService.ReadAsync)}:Id";
      realm = await _realmService.ReadAsync(realm.Id, cancellationToken: cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_realmService.ReadAsync)}:UniqueSlug";
      realm = await _realmService.ReadAsync(uniqueSlug: realm.UniqueSlug, cancellationToken: cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      _context.Realm = realm;
    }
    catch (Exception exception)
    {
      _context.Fail(name, exception);
    }

    return !_context.HasFailed;
  }
}
