using Logitar.Data;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Settings;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortalDb = Logitar.Portal.EntityFrameworkCore.Relational.PortalDb;

namespace Logitar.Portal.Application.Realms.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class DeleteRealmCommandTests : IntegrationTests
{
  private readonly IRealmRepository _realmRepository;
  private readonly IUserRepository _userRepository;

  private readonly Realm _realm;

  public DeleteRealmCommandTests() : base()
  {
    _realmRepository = ServiceProvider.GetRequiredService<IRealmRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();

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

  [Fact(DisplayName = "It should delete an existing realm.")]
  public async Task It_should_delete_an_existing_realm()
  {
    UniqueNameUnit uniqueName = new(new ReadOnlyUniqueNameSettings(), UsernameString);
    TenantId tenantId = new(_realm.Id.Value);
    UserAggregate user = new(uniqueName, tenantId);
    await _userRepository.SaveAsync(user);

    DeleteRealmCommand command = new(_realm.Id.ToGuid());
    RealmModel? realm = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(realm);
    Assert.Equal(_realm.Id.ToGuid(), realm.Id);

    Assert.Empty(await _userRepository.LoadAsync(tenantId));
  }

  [Fact(DisplayName = "It should return null when the realm cannot be found.")]
  public async Task It_should_return_null_when_the_realm_cannot_be_found()
  {
    DeleteRealmCommand command = new(Guid.NewGuid());
    RealmModel? realm = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(realm);
  }
}
