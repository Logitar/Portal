using Logitar.Data;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortalDb = Logitar.Portal.EntityFrameworkCore.Relational.PortalDb;

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

  [Theory(DisplayName = "It should create a new realm.")]
  [InlineData(null)]
  [InlineData("4f306445-b9b7-42b1-8ee6-caa025cdb540")]
  public async Task It_should_create_a_new_realm(string? idValue)
  {
    CreateRealmPayload payload = new("tests", secret: string.Empty);
    if (idValue != null)
    {
      payload.Id = Guid.Parse(idValue);
    }
    payload.CustomAttributes.Add(new("MfaMode", "MagicLink"));
    CreateRealmCommand command = new(payload);

    RealmModel realm = await ActivityPipeline.ExecuteAsync(command);
    if (payload.Id.HasValue)
    {
      Assert.Equal(payload.Id.Value, realm.Id);
    }
    Assert.Equal(payload.UniqueSlug, realm.UniqueSlug);
    Assert.NotEmpty(realm.Secret);
    Assert.Equal(payload.CustomAttributes, realm.CustomAttributes);
  }

  [Fact(DisplayName = "It should throw IdAlreadyUsedException when the ID is already taken.")]
  public async Task It_should_throw_IdAlreadyUsedException_when_the_Id_is_already_taken()
  {
    Realm realm = new(new Slug("new-realm"));
    await _realmRepository.SaveAsync(realm);

    CreateRealmPayload payload = new(realm.UniqueSlug.Value, realm.Secret.Value)
    {
      Id = realm.Id.ToGuid()
    };
    CreateRealmCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<IdAlreadyUsedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(payload.Id.Value, exception.Id);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw UniqueSlugAlreadyUsedException when the unique slug is already used.")]
  public async Task It_should_throw_UniqueSlugAlreadyUsedException_when_the_unique_slug_is_already_used()
  {
    Realm realm = new(new Slug("tests"));
    await _realmRepository.SaveAsync(realm);

    CreateRealmPayload payload = new(realm.UniqueSlug.Value, secret: string.Empty);
    CreateRealmCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UniqueSlugAlreadyUsedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(realm.UniqueSlug.Value, exception.Slug);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateRealmPayload payload = new(uniqueSlug: "", secret: string.Empty)
    {
      Id = Guid.Empty
    };
    CreateRealmCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));

    Assert.Equal(3, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Id.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "UniqueSlug");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "SlugValidator" && e.PropertyName == "UniqueSlug");
  }
}
