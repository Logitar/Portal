using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Realms.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchRealmsQueryTests : IntegrationTests
{
  private readonly IRealmRepository _realmRepository;

  public SearchRealmsQueryTests() : base()
  {
    _realmRepository = ServiceProvider.GetRequiredService<IRealmRepository>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [PortalDb.Realms.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should return empty results when no realm did match.")]
  public async Task It_should_return_empty_results_when_no_realm_did_match()
  {
    SearchRealmsPayload payload = new();
    payload.Search.Terms.Add(new SearchTerm("%test%"));
    SearchRealmsQuery query = new(payload);
    SearchResults<RealmModel> results = await ActivityPipeline.ExecuteAsync(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    Realm notMatching = new(new UniqueSlugUnit("tests"));
    Realm notInIds = new(new UniqueSlugUnit("realm-not-in-ids"));
    Realm realm1 = new(new UniqueSlugUnit("realm-1"));
    Realm realm2 = new(new UniqueSlugUnit("realm-2"));
    await _realmRepository.SaveAsync([notMatching, notInIds, realm1, realm2]);

    SearchRealmsPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    IEnumerable<Guid> realmIds = (await _realmRepository.LoadAsync()).Select(realm => realm.Id.ToGuid());
    payload.Ids.AddRange(realmIds);
    payload.Ids.Add(Guid.NewGuid());
    payload.Ids.Remove(notInIds.Id.ToGuid());
    payload.Search.Terms.Add(new SearchTerm("real%"));
    payload.Search.Operator = SearchOperator.Or;
    payload.Sort.Add(new RealmSortOption(RealmSort.DisplayName, isDescending: false));
    SearchRealmsQuery query = new(payload);
    SearchResults<RealmModel> results = await ActivityPipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    RealmModel realm = Assert.Single(results.Items);
    Assert.Equal(realm2.Id.ToGuid(), realm.Id);
  }
}
