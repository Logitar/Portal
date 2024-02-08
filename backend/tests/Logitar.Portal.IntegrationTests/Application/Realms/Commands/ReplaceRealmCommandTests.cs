using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Realms.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceRealmCommandTests : IntegrationTests
{
  private readonly IRealmRepository _realmRepository;

  private readonly RealmAggregate _realm;

  public ReplaceRealmCommandTests() : base()
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

  [Fact(DisplayName = "It should replace an existing realm.")]
  public async Task It_should_replace_an_existing_realm()
  {
    string oldSecret = _realm.Secret.Value;

    _realm.DefaultLocale = new LocaleUnit("fr");
    _realm.Update();
    await _realmRepository.SaveAsync(_realm);
    long version = _realm.Version;

    _realm.DefaultLocale = new LocaleUnit("fr-CA");
    _realm.Update();
    await _realmRepository.SaveAsync(_realm);

    ReplaceRealmPayload payload = new(_realm.UniqueSlug.Value, secret: "  ")
    {
      DisplayName = "Tests",
      Description = "  This is a test realm.  ",
      DefaultLocale = "fr",
      Url = $"https://www.{Faker.Internet.DomainName()}/"
    };
    payload.CustomAttributes.Add(new("Guid", Guid.NewGuid().ToString()));
    ReplaceRealmCommand command = new(_realm.Id.AggregateId.ToGuid(), payload, version);
    Realm? realm = await Mediator.Send(command);
    Assert.NotNull(realm);

    Assert.Equal(command.Id, realm.Id);
    Assert.Equal(_realm.UniqueSlug.Value, realm.UniqueSlug);
    Assert.Equal(payload.DisplayName, realm.DisplayName);
    Assert.Equal(payload.Description.Trim(), realm.Description);
    Assert.Equal("fr-CA", realm.DefaultLocale);
    Assert.NotEqual(oldSecret, realm.Secret);
    Assert.Equal(payload.Url, realm.Url);
  }

  [Fact(DisplayName = "It should return null when the realm cannot be found.")]
  public async Task It_should_return_null_when_the_realm_cannot_be_found()
  {
    ReplaceRealmPayload payload = new("tests", secret: string.Empty);
    ReplaceRealmCommand command = new(Guid.NewGuid(), payload, Version: null);
    Realm? realm = await Mediator.Send(command);
    Assert.Null(realm);
  }

  [Fact(DisplayName = "It should throw UniqueSlugAlreadySlugException when the unique slug is already used.")]
  public async Task It_should_throw_UniqueSlugAlreadySlugException_when_the_unique_slug_is_already_used()
  {
    RealmAggregate realm = new(new UniqueSlugUnit("other"));
    await _realmRepository.SaveAsync(realm);

    ReplaceRealmPayload payload = new(_realm.UniqueSlug.Value.ToUpper(), secret: string.Empty);
    ReplaceRealmCommand command = new(realm.Id.AggregateId.ToGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<UniqueSlugAlreadyUsedException>(async () => await Mediator.Send(command));
    Assert.Equal(_realm.UniqueSlug.Value, exception.UniqueSlug.Value, ignoreCase: true);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceRealmPayload payload = new("hello--world!", secret: string.Empty);
    ReplaceRealmCommand command = new(Guid.NewGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.Equal("UniqueSlug", exception.Errors.Single().PropertyName);
  }
}
