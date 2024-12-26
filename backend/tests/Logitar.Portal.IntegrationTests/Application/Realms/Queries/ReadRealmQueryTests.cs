using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Realms.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadRealmQueryTests : IntegrationTests
{
  private readonly IRealmRepository _realmRepository;

  private readonly Realm _realm;

  public ReadRealmQueryTests() : base()
  {
    _realmRepository = ServiceProvider.GetRequiredService<IRealmRepository>();

    _realm = new(new Slug("tests"));
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

    await _realmRepository.SaveAsync(_realm);
  }

  [Fact(DisplayName = "It should return null when the realm cannot be found.")]
  public async Task It_should_return_null_when_the_realm_cannot_be_found()
  {
    ReadRealmQuery query = new(Guid.NewGuid(), UniqueSlug: null);
    RealmModel? realm = await ActivityPipeline.ExecuteAsync(query);
    Assert.Null(realm);
  }

  [Fact(DisplayName = "It should return the realm found by ID.")]
  public async Task It_should_return_the_realm_found_by_Id()
  {
    ReadRealmQuery query = new(_realm.Id.ToGuid(), _realm.UniqueSlug.Value);
    RealmModel? realm = await ActivityPipeline.ExecuteAsync(query);
    Assert.NotNull(realm);
    Assert.Equal(_realm.Id.ToGuid(), realm.Id);
  }

  [Fact(DisplayName = "It should return the realm found by unique slug.")]
  public async Task It_should_return_the_realm_found_by_unique_slug()
  {
    ReadRealmQuery query = new(Id: null, _realm.UniqueSlug.Value);
    RealmModel? realm = await ActivityPipeline.ExecuteAsync(query);
    Assert.NotNull(realm);
    Assert.Equal(_realm.Id.ToGuid(), realm.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when there are too many results.")]
  public async Task It_should_throw_TooManyResultsException_when_there_are_too_many_results()
  {
    Realm realm = new(new Slug("other"));
    await _realmRepository.SaveAsync(realm);

    ReadRealmQuery query = new(_realm.Id.ToGuid(), "  OthEr  ");
    var exception = await Assert.ThrowsAsync<TooManyResultsException<RealmModel>>(async () => await ActivityPipeline.ExecuteAsync(query));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
