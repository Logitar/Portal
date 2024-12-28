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

  [Theory(DisplayName = "It should create a new dictionary.")]
  [InlineData(null)]
  [InlineData("fcc06e6b-8a37-4410-998f-fe8fa20a84ec")]
  public async Task It_should_create_a_new_dictionary(string? idValue)
  {
    SetRealm();

    CreateDictionaryPayload payload = new(Faker.Locale);
    if (idValue != null)
    {
      payload.Id = Guid.Parse(idValue);
    }
    CreateDictionaryCommand command = new(payload);
    DictionaryModel dictionary = await ActivityPipeline.ExecuteAsync(command);

    Assert.Equal(payload.Locale, dictionary.Locale.Code);
    if (payload.Id.HasValue)
    {
      Assert.Equal(payload.Id.Value, dictionary.Id);
    }
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

  [Fact(DisplayName = "It should throw IdAlreadyUsedException when the ID is already taken.")]
  public async Task It_should_throw_IdAlreadyUsedException_when_the_Id_is_already_taken()
  {
    Dictionary dictionary = new(new Locale("fr"));
    await _dictionaryRepository.SaveAsync(dictionary);

    CreateDictionaryPayload payload = new(dictionary.Locale.Code)
    {
      Id = dictionary.EntityId.ToGuid()
    };
    CreateDictionaryCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<IdAlreadyUsedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(payload.Id.Value, exception.Id);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateDictionaryPayload payload = new(locale: string.Empty)
    {
      Id = Guid.Empty
    };
    CreateDictionaryCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));

    Assert.Equal(3, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Id.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Locale");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "LocaleValidator" && e.PropertyName == "Locale");
  }
}
