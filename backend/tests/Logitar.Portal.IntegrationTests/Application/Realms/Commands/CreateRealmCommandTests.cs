using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Realms.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateRealmCommandTests : IntegrationTests
{
  private readonly IRealmRepository _realmRepository;

  public CreateRealmCommandTests() : base()
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

  [Fact(DisplayName = "It should create a new realm.")]
  public async Task It_should_create_a_new_realm()
  {
    CreateRealmPayload payload = new("tests", secret: string.Empty);
    payload.CustomAttributes.Add(new("MfaMode", "MagicLink"));
    CreateRealmCommand command = new(payload);

    Realm realm = await Mediator.Send(command);
    Assert.Equal(payload.UniqueSlug, realm.UniqueSlug);
    Assert.NotEmpty(realm.Secret);
    Assert.Equal(payload.CustomAttributes, realm.CustomAttributes);
  }

  [Fact(DisplayName = "It should throw UniqueSlugAlreadyUsedException when the unique slug is already used.")]
  public async Task It_should_throw_UniqueSlugAlreadyUsedException_when_the_unique_slug_is_already_used()
  {
    RealmAggregate realm = new(new UniqueSlugUnit("tests"));
    await _realmRepository.SaveAsync(realm);

    CreateRealmPayload payload = new(realm.UniqueSlug.Value, secret: string.Empty);
    CreateRealmCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UniqueSlugAlreadyUsedException>(async () => await Mediator.Send(command));
    Assert.Equal(realm.UniqueSlug, exception.UniqueSlug);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateRealmPayload payload = new(uniqueSlug: "", secret: string.Empty);
    CreateRealmCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.All(exception.Errors, e => Assert.Equal("UniqueSlug", e.PropertyName));
  }
}
