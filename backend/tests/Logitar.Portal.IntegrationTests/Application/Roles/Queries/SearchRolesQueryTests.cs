using Logitar.Data;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Roles;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IdentityDb = Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;

namespace Logitar.Portal.Application.Roles.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchRolesQueryTests : IntegrationTests
{
  private readonly IRoleRepository _roleRepository;

  private readonly Role _role;

  public SearchRolesQueryTests() : base()
  {
    _roleRepository = ServiceProvider.GetRequiredService<IRoleRepository>();

    ReadOnlyUniqueNameSettings uniqueNameSettings = new();
    UniqueName uniqueName = new(uniqueNameSettings, "admin");
    _role = new(uniqueName);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [IdentityDb.Roles.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    await _roleRepository.SaveAsync(_role);
  }

  [Fact(DisplayName = "It should return empty results when no role did match.")]
  public async Task It_should_return_empty_results_when_no_role_did_match()
  {
    SearchRolesPayload payload = new();
    payload.Search.Terms.Add(new SearchTerm("%test%"));
    SearchRolesQuery query = new(payload);
    SearchResults<RoleModel> results = await ActivityPipeline.ExecuteAsync(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    Role admin = new(new UniqueName(Realm.UniqueNameSettings, "admin"), actorId: null, RoleId.NewId(TenantId));
    Role geoGuesser = new(new UniqueName(Realm.UniqueNameSettings, "geo_guesser"), actorId: null, RoleId.NewId(TenantId));
    Role guest = new(new UniqueName(Realm.UniqueNameSettings, "guest"), actorId: null, RoleId.NewId(TenantId));
    Role minister = new(new UniqueName(Realm.UniqueNameSettings, "minister"), actorId: null, RoleId.NewId(TenantId));
    await _roleRepository.SaveAsync([admin, geoGuesser, guest, minister]);

    SetRealm();

    SearchRolesPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    IEnumerable<Guid> roleIds = (await _roleRepository.LoadAsync()).Select(role => role.EntityId.ToGuid());
    payload.Ids.AddRange(roleIds);
    payload.Ids.Add(Guid.NewGuid());
    payload.Ids.Remove(minister.EntityId.ToGuid());
    payload.Search.Terms.Add(new SearchTerm("%min%"));
    payload.Search.Terms.Add(new SearchTerm("gues%"));
    payload.Search.Operator = SearchOperator.Or;
    payload.Sort.Add(new RoleSortOption(RoleSort.DisplayName, isDescending: false));
    SearchRolesQuery query = new(payload);
    SearchResults<RoleModel> results = await ActivityPipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    RoleModel role = Assert.Single(results.Items);
    Assert.Equal(guest.EntityId.ToGuid(), role.Id);
  }
}
