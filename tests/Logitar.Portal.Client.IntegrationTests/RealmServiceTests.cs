using Logitar.Portal.Client.Implementations;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Client;

internal class RealmServiceTests
{
  private readonly TestContext _context;
  private readonly IRealmService _realmService;

  public RealmServiceTests(TestContext context, IRealmService realmService)
  {
    _context = context;
    _realmService = realmService;
  }

  public async Task<Realm?> ExecuteAsync(CancellationToken cancellationToken = default)
  {
    string name = string.Empty;
    try
    {
      name = string.Join('.', nameof(RealmService), nameof(RealmService.CreateAsync));
      CreateRealmInput input = new()
      {
        UniqueName = "test"
      };
      Realm realm = await _realmService.CreateAsync(input, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(RealmService), nameof(RealmService.UpdateAsync));
      UpdateRealmInput update = new()
      {
        DisplayName = "Test Realm",
        Description = "    ",
        DefaultLocale = "en-CA",
        Url = "https://www.test.com/",
        RequireConfirmedAccount = true,
        RequireUniqueEmail = true
      };
      realm = await _realmService.UpdateAsync(realm.Id, update, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(RealmService), nameof(RealmService.GetAsync));
      realm = (await _realmService.GetAsync(search: null, cancellationToken: cancellationToken)).Items.Single();
      _context.Succeed(name);

      name = string.Join('.', nameof(RealmService), $"{nameof(RealmService.GetAsync)}(id)");
      realm = await _realmService.GetAsync(realm.Id, cancellationToken: cancellationToken)
        ?? throw new InvalidOperationException("The result cannot be null.");
      _context.Succeed(name);

      name = string.Join('.', nameof(RealmService), nameof(RealmService.DeleteAsync));
      Guid deleteId = (await _realmService.CreateAsync(new CreateRealmInput { UniqueName = "delete-me" }, cancellationToken)).Id;
      _ = await _realmService.DeleteAsync(deleteId, cancellationToken);
      _context.Succeed(name);

      return realm;
    }
    catch (Exception exception)
    {
      _context.Fail(name, exception);

      return null;
    }
  }
}
