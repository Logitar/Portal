using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Identity.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Realms.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateRealmCommandTests : IntegrationTests
{
  private readonly IRealmRepository _realmRepository;

  private readonly RealmAggregate _realm;

  public UpdateRealmCommandTests() : base()
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
      ICommand command = CreateDeleteBuilder(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    await _realmRepository.SaveAsync(_realm);
  }

  [Fact(DisplayName = "It should return null when the realm cannot be found.")]
  public async Task It_should_return_null_when_the_realm_cannot_be_found()
  {
    UpdateRealmPayload payload = new();
    UpdateRealmCommand command = new(Guid.NewGuid(), payload);
    Realm? realm = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(realm);
  }

  [Fact(DisplayName = "It should throw UniqueSlugAlreadyUsedException when the unique slug is already used.")]
  public async Task It_should_throw_UniqueSlugAlreadyUsedException_when_the_unique_slug_is_already_used()
  {
    RealmAggregate realm = new(new UniqueSlugUnit("other"));
    await _realmRepository.SaveAsync(realm);

    UpdateRealmPayload payload = new()
    {
      UniqueSlug = _realm.UniqueSlug.Value
    };
    UpdateRealmCommand command = new(realm.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<UniqueSlugAlreadyUsedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(_realm.UniqueSlug, exception.UniqueSlug);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateRealmPayload payload = new()
    {
      UniqueSlug = "hello--world!"
    };
    UpdateRealmCommand command = new(Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("UniqueSlug", exception.Errors.Single().PropertyName);
  }

  [Fact(DisplayName = "It should update an existing realm.")]
  public async Task It_should_update_an_existing_realm()
  {
    UpdateRealmPayload payload = new()
    {
      DisplayName = new Modification<string>("Tests"),
      Description = new Modification<string>("  This is a test realm.  "),
      DefaultLocale = new Modification<string>("fr"),
      Url = new Modification<string>($"https://www.{Faker.Internet.DomainName()}/")
    };
    UpdateRealmCommand command = new(_realm.Id.ToGuid(), payload);
    Realm? realm = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(realm);

    Assert.Equal(command.Id, realm.Id);
    Assert.Equal(payload.DisplayName.Value, realm.DisplayName);
    Assert.Equal(payload.Description.Value?.Trim(), realm.Description);
    Assert.Equal(payload.DefaultLocale.Value, realm.DefaultLocale?.Code);
    Assert.Equal(payload.Url.Value, realm.Url);
  }
}
