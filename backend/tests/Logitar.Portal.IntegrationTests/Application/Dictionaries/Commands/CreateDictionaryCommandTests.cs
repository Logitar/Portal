using Logitar.Data;
using Logitar.Identity.Core;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortalDb = Logitar.Portal.EntityFrameworkCore.Relational.PortalDb;

namespace Logitar.Portal.Application.Dictionaries.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateDictionaryCommandTests : IntegrationTests
{
  private readonly IDictionaryRepository _dictionaryRepository;

  private readonly Dictionary _dictionary;

  public CreateDictionaryCommandTests() : base()
  {
    _dictionaryRepository = ServiceProvider.GetRequiredService<IDictionaryRepository>();

    _dictionary = new(new Locale(Faker.Locale));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [PortalDb.Dictionaries.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    await _dictionaryRepository.SaveAsync(_dictionary);
  }

  [Fact(DisplayName = "It should create a new dictionary.")]
  public async Task It_should_create_a_new_dictionary()
  {
    SetRealm();

    CreateDictionaryPayload payload = new(Faker.Locale);
    CreateDictionaryCommand command = new(payload);
    DictionaryModel dictionary = await ActivityPipeline.ExecuteAsync(command);

    Assert.Equal(payload.Locale, dictionary.Locale.Code);
    Assert.Equal(0, dictionary.EntryCount);
    Assert.Empty(dictionary.Entries);
    Assert.Same(Realm, dictionary.Realm);
  }

  [Fact(DisplayName = "It should throw DictionaryAlreadyExistsException when the dictionary already exists.")]
  public async Task It_should_throw_DictionaryAlreadyExistsException_when_the_dictionary_already_exists()
  {
    CreateDictionaryPayload payload = new(Faker.Locale);
    CreateDictionaryCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<DictionaryAlreadyExistsException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Null(exception.TenantId);
    Assert.Equal(payload.Locale, exception.Locale);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateDictionaryPayload payload = new(locale: string.Empty);
    CreateDictionaryCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.All(exception.Errors, e => Assert.Equal("Locale", e.PropertyName));
  }
}
