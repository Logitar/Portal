using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Identity.Domain.Roles;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Roles.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchRolesQueryTests : IntegrationTests
{
  private readonly IRoleRepository _roleRepository;

  private readonly RoleAggregate _role;

  public SearchRolesQueryTests() : base()
  {
    _roleRepository = ServiceProvider.GetRequiredService<IRoleRepository>();

    ReadOnlyUniqueNameSettings uniqueNameSettings = new();
    UniqueNameUnit uniqueName = new(uniqueNameSettings, "admin");
    _role = new(uniqueName);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [IdentityDb.Roles.Table];
    foreach (TableId table in tables)
    {
      ICommand command = SqlServerDeleteBuilder.From(table).Build();
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
    SearchResults<Role> results = await Mediator.Send(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    RoleAggregate admin = new(new UniqueNameUnit(Realm.UniqueNameSettings, "admin"), TenantId);
    RoleAggregate geoGuesser = new(new UniqueNameUnit(Realm.UniqueNameSettings, "geo_guesser"), TenantId);
    RoleAggregate guest = new(new UniqueNameUnit(Realm.UniqueNameSettings, "guest"), TenantId);
    RoleAggregate minister = new(new UniqueNameUnit(Realm.UniqueNameSettings, "minister"), TenantId);
    await _roleRepository.SaveAsync([admin, geoGuesser, guest, minister]);

    SetRealm();

    SearchRolesPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    IEnumerable<Guid> roleIds = (await _roleRepository.LoadAsync()).Select(role => role.Id.ToGuid());
    payload.Ids.AddRange(roleIds);
    payload.Ids.Add(Guid.NewGuid());
    payload.Ids.Remove(minister.Id.ToGuid());
    payload.Search.Terms.Add(new SearchTerm("%min%"));
    payload.Search.Terms.Add(new SearchTerm("gues%"));
    payload.Search.Operator = SearchOperator.Or;
    payload.Sort.Add(new RoleSortOption(RoleSort.DisplayName, isDescending: false));
    SearchRolesQuery query = new(payload);
    SearchResults<Role> results = await Mediator.Send(query);

    Assert.Equal(2, results.Total);
    Role role = Assert.Single(results.Items);
    Assert.Equal(guest.Id.ToGuid(), role.Id);
  }
}
