using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Dictionaries.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceDictionaryCommandTests : IntegrationTests
{
  private readonly IDictionaryRepository _dictionaryRepository;

  private readonly DictionaryAggregate _dictionary;

  public ReplaceDictionaryCommandTests() : base()
  {
    _dictionaryRepository = ServiceProvider.GetRequiredService<IDictionaryRepository>();

    _dictionary = new(new LocaleUnit(Faker.Locale));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [PortalDb.Dictionaries.Table];
    foreach (TableId table in tables)
    {
      ICommand command = SqlServerDeleteBuilder.From(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    await _dictionaryRepository.SaveAsync(_dictionary);
  }

  [Fact(DisplayName = "It should replace an existing dictionary.")]
  public async Task It_should_replace_an_existing_dictionary()
  {
    _dictionary.SetEntry("ecarlate", "scarlet");
    _dictionary.SetEntry("cyan", "cyan");
    _dictionary.Update();
    await _dictionaryRepository.SaveAsync(_dictionary);
    long version = _dictionary.Version;

    _dictionary.SetEntry("emeraude", "emerald");
    _dictionary.SetEntry("cyan", "light blue");
    _dictionary.Update();
    await _dictionaryRepository.SaveAsync(_dictionary);

    ReplaceDictionaryPayload payload = new(Faker.Locale);
    payload.Entries.Add(new("cyan", "cyan"));
    payload.Entries.Add(new("or", "golden"));
    ReplaceDictionaryCommand command = new(_dictionary.Id.ToGuid(), payload, version);
    Dictionary? dictionary = await Mediator.Send(command);
    Assert.NotNull(dictionary);

    Assert.Equal(payload.Locale, dictionary.Locale.Code);

    Assert.Equal(3, dictionary.EntryCount);
    Assert.Equal(3, dictionary.Entries.Count);
    Assert.Contains(dictionary.Entries, c => c.Key == "cyan" && c.Value == "light blue");
    Assert.Contains(dictionary.Entries, c => c.Key == "emeraude" && c.Value == "emerald");
    Assert.Contains(dictionary.Entries, c => c.Key == "or" && c.Value == "golden");
  }

  [Fact(DisplayName = "It should return null when the dictionary cannot be found.")]
  public async Task It_should_return_null_when_the_dictionary_cannot_be_found()
  {
    ReplaceDictionaryPayload payload = new(Faker.Locale);
    ReplaceDictionaryCommand command = new(Guid.NewGuid(), payload, Version: null);
    Dictionary? dictionary = await Mediator.Send(command);
    Assert.Null(dictionary);
  }

  [Fact(DisplayName = "It should return null when the dictionary is in another tenant.")]
  public async Task It_should_return_null_when_the_dictionary_is_in_another_tenant()
  {
    SetRealm();

    ReplaceDictionaryPayload payload = new(Faker.Locale);
    ReplaceDictionaryCommand command = new(_dictionary.Id.ToGuid(), payload, Version: null);
    Dictionary? result = await Mediator.Send(command);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw DictionaryAlreadyExistsException when the dictionary already exists.")]
  public async Task It_should_throw_DictionaryAlreadyExistsException_when_the_dictionary_already_exists()
  {
    DictionaryAggregate dictionary = new(new LocaleUnit("fr"));
    await _dictionaryRepository.SaveAsync(dictionary);

    ReplaceDictionaryPayload payload = new(Faker.Locale.ToUpper());
    ReplaceDictionaryCommand command = new(dictionary.Id.ToGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<DictionaryAlreadyExistsException>(async () => await Mediator.Send(command));
    Assert.Null(exception.TenantId);
    Assert.Equal(payload.Locale, exception.Locale.Code, ignoreCase: true);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceDictionaryPayload payload = new("/!\\");
    ReplaceDictionaryCommand command = new(Guid.NewGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.Equal("Locale", exception.Errors.Single().PropertyName);
  }
}
