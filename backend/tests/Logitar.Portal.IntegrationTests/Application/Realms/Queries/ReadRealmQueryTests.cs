using Logitar.Data;
using Logitar.Data.SqlServer;
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

  private readonly RealmAggregate _realm;

  public ReadRealmQueryTests()
  {
    _realmRepository = ServiceProvider.GetRequiredService<IRealmRepository>();

    _realm = new(new UniqueSlugUnit("tests"));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [PortalDb.Realms.Table];
    foreach (TableId table in tables)
    {
      ICommand command = SqlServerDeleteBuilder.From(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    await _realmRepository.SaveAsync(_realm);
  }

  [Fact(DisplayName = "It should return null when the realm cannot be found.")]
  public async Task It_should_return_null_when_the_realm_cannot_be_found()
  {
    ReadRealmQuery query = new(Guid.NewGuid(), UniqueSlug: null);
    Realm? realm = await Mediator.Send(query);
    Assert.Null(realm);
  }

  [Fact(DisplayName = "It should return the realm found by ID.")]
  public async Task It_should_return_the_realm_found_by_Id()
  {
    ReadRealmQuery query = new(_realm.Id.AggregateId.ToGuid(), _realm.UniqueSlug.Value);
    Realm? realm = await Mediator.Send(query);
    Assert.NotNull(realm);
    Assert.Equal(_realm.Id.AggregateId.ToGuid(), realm.Id);
  }

  [Fact(DisplayName = "It should return the realm found by unique slug.")]
  public async Task It_should_return_the_realm_found_by_unique_slug()
  {
    ReadRealmQuery query = new(Id: null, _realm.UniqueSlug.Value);
    Realm? realm = await Mediator.Send(query);
    Assert.NotNull(realm);
    Assert.Equal(_realm.Id.AggregateId.ToGuid(), realm.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when there are too many results.")]
  public async Task It_should_throw_TooManyResultsException_when_there_are_too_many_results()
  {
    RealmAggregate realm = new(new UniqueSlugUnit("other"));
    await _realmRepository.SaveAsync(realm);

    ReadRealmQuery query = new(_realm.Id.AggregateId.ToGuid(), "  OthEr  ");
    var exception = await Assert.ThrowsAsync<TooManyResultsException<Realm>>(async () => await Mediator.Send(query));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
