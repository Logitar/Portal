using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Application.Dictionaries;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.Domain.Realms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

[Trait(Traits.Category, Categories.Integration)]
public class DictionaryServiceTests : IntegrationTests, IAsyncLifetime
{
  private readonly IDictionaryService _dictionaryService;

  private readonly RealmAggregate _realm;
  private readonly ReadOnlyLocale _locale = new("fr");
  private readonly DictionaryAggregate _dictionary;

  public DictionaryServiceTests() : base()
  {
    _dictionaryService = ServiceProvider.GetRequiredService<IDictionaryService>();

    _realm = new("logitar");

    _locale = new(Faker.Locale);

    _dictionary = new(_locale, _realm.Id.Value);
    _dictionary.SetEntry("Red", "Rouge");
    _dictionary.SetEntry("Green", "Vert");
    _dictionary.SetEntry("Blue", "Bleu");
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await AggregateRepository.SaveAsync(new AggregateRoot[] { _realm, _dictionary });
  }

  [Fact(DisplayName = "CreateAsync: it should create a dictionary.")]
  public async Task CreateAsync_it_should_create_a_dictionary()
  {
    CreateDictionaryPayload payload = new()
    {
      Realm = $"  {_realm.UniqueSlug.ToUpper()}  ",
      Locale = "  es  ",
      Entries = new DictionaryEntry[]
      {
        new("  Red  ", "  Rojo  "),
        new("Green", "Verde"),
        new("Blue", "Azul")
      }
    };

    Dictionary? dictionary = await _dictionaryService.CreateAsync(payload);
    Assert.NotNull(dictionary);

    Assert.NotEqual(Guid.Empty, dictionary.Id);
    Assert.Equal(Actor, dictionary.CreatedBy);
    AssertIsNear(dictionary.CreatedOn);
    Assert.Equal(Actor, dictionary.UpdatedBy);
    AssertIsNear(dictionary.UpdatedOn);
    Assert.True(dictionary.Version >= 1);

    Assert.Equal(payload.Locale.Trim(), dictionary.Locale.Code);

    Assert.Equal(3, dictionary.Entries.Count());
    Assert.Equal(3, dictionary.EntryCount);
    Assert.Contains(dictionary.Entries, customAttribute => customAttribute.Key == "Red" && customAttribute.Value == "Rojo");
    Assert.Contains(dictionary.Entries, customAttribute => customAttribute.Key == "Green" && customAttribute.Value == "Verde");
    Assert.Contains(dictionary.Entries, customAttribute => customAttribute.Key == "Blue" && customAttribute.Value == "Azul");

    Assert.NotNull(dictionary.Realm);
    Assert.Equal(_realm.Id.ToGuid(), dictionary.Realm.Id);
  }

  [Fact(DisplayName = "CreateAsync: it should throw AggregateNotFoundException when the realm is not found.")]
  public async Task CreateAsync_it_should_throw_AggregateNotFoundException_when_the_realm_is_not_found()
  {
    CreateDictionaryPayload payload = new()
    {
      Realm = Guid.Empty.ToString(),
      Locale = _locale.Code
    };

    var exception = await Assert.ThrowsAsync<AggregateNotFoundException<RealmAggregate>>(async () => await _dictionaryService.CreateAsync(payload));
    Assert.Equal(payload.Realm, exception.Id);
    Assert.Equal(nameof(payload.Realm), exception.PropertyName);
  }

  [Fact(DisplayName = "CreateAsync: it should throw DictionaryAlreadyExistingException when the dictionary already exists.")]
  public async Task CreateAsync_it_should_throw_DictionaryAlreadyExistingException_when_the_dictionary_already_exists()
  {
    CreateDictionaryPayload payload = new()
    {
      Realm = _realm.UniqueSlug,
      Locale = _locale.Code
    };

    var exception = await Assert.ThrowsAsync<DictionaryAlreadyExistingException>(async () => await _dictionaryService.CreateAsync(payload));
    Assert.Equal(_realm.Id.Value, exception.TenantId);
    Assert.Equal(payload.Locale, exception.LocaleCode);
    Assert.Equal("Realm,Locale", exception.PropertyName);
  }

  [Fact(DisplayName = "DeleteAsync: it should delete the dictionary.")]
  public async Task DeleteAsync_it_should_delete_the_dictionary()
  {
    ReadOnlyLocale locale = new("es");
    DictionaryAggregate other = new(locale, _realm.Id.Value);
    await AggregateRepository.SaveAsync(other);

    Dictionary? dictionary = await _dictionaryService.DeleteAsync(_dictionary.Id.ToGuid());

    Assert.NotNull(dictionary);
    Assert.Equal(_dictionary.Id.ToGuid(), dictionary.Id);

    Assert.Null(await PortalContext.Dictionaries.SingleOrDefaultAsync(x => x.AggregateId == _dictionary.Id.Value));
    Assert.NotNull(await PortalContext.Dictionaries.SingleOrDefaultAsync(x => x.AggregateId == other.Id.Value));
  }

  [Fact(DisplayName = "DeleteAsync: it should return null when the dictionary is not found.")]
  public async Task DeleteAsync_it_should_return_null_when_the_dictionary_is_not_found()
  {
    Assert.Null(await _dictionaryService.DeleteAsync(Guid.Empty));
  }

  [Fact(DisplayName = "ReadAsync: it should return null when the dictionary is not found.")]
  public async Task ReadAsync_it_should_return_null_when_the_dictionary_is_not_found()
  {
    Assert.Null(await _dictionaryService.ReadAsync(Guid.Empty, _realm.UniqueSlug, "jp"));
  }

  [Fact(DisplayName = "ReadAsync: it should return the dictionary found by ID.")]
  public async Task ReadAsync_it_should_return_the_dictionary_found_by_Id()
  {
    Dictionary? dictionary = await _dictionaryService.ReadAsync(_dictionary.Id.ToGuid());
    Assert.NotNull(dictionary);
    Assert.Equal(_dictionary.Id.ToGuid(), dictionary.Id);
  }

  [Fact(DisplayName = "ReadAsync: it should return the dictionary found by realm and locale.")]
  public async Task ReadAsync_it_should_return_the_dictionary_found_by_realm_and_locale()
  {
    Dictionary? dictionary = await _dictionaryService.ReadAsync(realm: $" {_realm.Id.ToGuid()} ", locale: $" {_locale.Code} ");
    Assert.NotNull(dictionary);
    Assert.Equal(_dictionary.Id.ToGuid(), dictionary.Id);
  }

  [Fact(DisplayName = "ReadAsync: it should throw TooManyResultsException when many dictionaries have been found.")]
  public async Task ReadAsync_it_should_throw_TooManyResultsException_when_many_dictionaries_have_been_found()
  {
    DictionaryAggregate dictionary = new(_locale);
    await AggregateRepository.SaveAsync(dictionary);

    var exception = await Assert.ThrowsAsync<TooManyResultsException<Dictionary>>(
      async () => await _dictionaryService.ReadAsync(dictionary.Id.ToGuid(), _realm.UniqueSlug, _dictionary.Locale.Code)
    );
    Assert.Equal(1, exception.Expected);
    Assert.Equal(2, exception.Actual);
  }

  [Fact(DisplayName = "ReplaceAsync: it should replace the dictionary.")]
  public async Task ReplaceAsync_it_should_replace_the_dictionary()
  {
    long version = _dictionary.Version;

    _dictionary.SetEntry("Jaune", "Yellow");
    await AggregateRepository.SaveAsync(_dictionary);

    ReplaceDictionaryPayload payload = new()
    {
      Entries = new DictionaryEntry[]
      {
        new("Red", "Rouge"),
        new("Blue", "Cyan")
      }
    };

    Dictionary? dictionary = await _dictionaryService.ReplaceAsync(_dictionary.Id.ToGuid(), payload, version);

    Assert.NotNull(dictionary);

    Assert.Equal(_dictionary.Id.ToGuid(), dictionary.Id);
    Assert.Equal(Guid.Empty, dictionary.CreatedBy.Id);
    AssertEqual(_dictionary.CreatedOn, dictionary.CreatedOn);
    Assert.Equal(Actor, dictionary.UpdatedBy);
    AssertIsNear(dictionary.UpdatedOn);
    Assert.True(dictionary.Version > 1);

    Assert.Equal(3, dictionary.Entries.Count());
    Assert.Equal(3, dictionary.EntryCount);
    Assert.Contains(dictionary.Entries, customAttribute => customAttribute.Key == "Red" && customAttribute.Value == "Rouge");
    Assert.Contains(dictionary.Entries, customAttribute => customAttribute.Key == "Blue" && customAttribute.Value == "Cyan");
    Assert.Contains(dictionary.Entries, customAttribute => customAttribute.Key == "Jaune" && customAttribute.Value == "Yellow");
  }

  [Fact(DisplayName = "ReplaceAsync: it should return null when the dictionary is not found.")]
  public async Task ReplaceAsync_it_should_return_null_when_the_dictionary_is_not_found()
  {
    ReplaceDictionaryPayload payload = new();
    Assert.Null(await _dictionaryService.ReplaceAsync(Guid.Empty, payload));
  }

  [Fact(DisplayName = "SearchAsync: it should return empty results when none are matching.")]
  public async Task SearchAsync_it_should_return_empty_results_when_none_are_matching()
  {
    SearchDictionariesPayload payload = new()
    {
      IdIn = new[] { Guid.Empty }
    };

    SearchResults<Dictionary> results = await _dictionaryService.SearchAsync(payload);

    Assert.Empty(results.Results);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "SearchAsync: it should return the correct results.")]
  public async Task SearchAsync_it_should_return_the_correct_results()
  {
    DictionaryAggregate notInRealm = new(_locale);
    DictionaryAggregate french = new(new ReadOnlyLocale("fr"), _realm.Id.Value);
    DictionaryAggregate dictionary1 = new(new ReadOnlyLocale("en-AU"), _realm.Id.Value);
    DictionaryAggregate dictionary2 = new(new ReadOnlyLocale("en-CA"), _realm.Id.Value);
    DictionaryAggregate dictionary3 = new(new ReadOnlyLocale("en-GB"), _realm.Id.Value);
    DictionaryAggregate dictionary4 = new(new ReadOnlyLocale("en-US"), _realm.Id.Value);
    await AggregateRepository.SaveAsync(new[] { notInRealm, french, dictionary1, dictionary2, dictionary3, dictionary4 });

    DictionaryAggregate[] dictionaries = new[] { dictionary1, dictionary2, dictionary3, dictionary4 }
      .OrderBy(x => x.Locale.Code).Skip(1).Take(2).ToArray();

    HashSet<Guid> ids = (await PortalContext.Dictionaries.AsNoTracking().ToArrayAsync())
      .Select(dictionary => new AggregateId(dictionary.AggregateId).ToGuid()).ToHashSet();
    ids.Remove(_dictionary.Id.ToGuid());

    SearchDictionariesPayload payload = new()
    {
      Search = new TextSearch
      {
        Operator = SearchOperator.Or,
        Terms = new SearchTerm[]
        {
          new("en%"),
          new(Guid.NewGuid().ToString())
        }
      },
      IdIn = ids,
      Realm = $" {_realm.UniqueSlug.ToUpper()} ",
      Sort = new DictionarySortOption[]
      {
        new(DictionarySort.Locale)
      },
      Skip = 1,
      Limit = 2
    };

    SearchResults<Dictionary> results = await _dictionaryService.SearchAsync(payload);

    Assert.Equal(dictionaries.Length, results.Results.Count());
    Assert.Equal(4, results.Total);

    for (int i = 0; i < dictionaries.Length; i++)
    {
      Assert.Equal(dictionaries[i].Id.ToGuid(), results.Results.ElementAt(i).Id);
    }
  }

  [Fact(DisplayName = "UpdateAsync: it should return null when the dictionary is not found.")]
  public async Task UpdateAsync_it_should_return_null_when_the_dictionary_is_not_found()
  {
    UpdateDictionaryPayload payload = new();
    Assert.Null(await _dictionaryService.UpdateAsync(Guid.Empty, payload));
  }

  [Fact(DisplayName = "UpdateAsync: it should update the dictionary.")]
  public async Task UpdateAsync_it_should_update_the_dictionary()
  {
    UpdateDictionaryPayload payload = new()
    {
      Entries = new DictionaryEntryModification[]
      {
        new("Yellow", "Jaune"),
        new("Blue", "Cyan"),
        new("Green", value: null)
      }
    };

    Dictionary? dictionary = await _dictionaryService.UpdateAsync(_dictionary.Id.ToGuid(), payload);

    Assert.NotNull(dictionary);

    Assert.Equal(_dictionary.Id.ToGuid(), dictionary.Id);
    Assert.Equal(Guid.Empty, dictionary.CreatedBy.Id);
    AssertEqual(_dictionary.CreatedOn, dictionary.CreatedOn);
    Assert.Equal(Actor, dictionary.UpdatedBy);
    AssertIsNear(dictionary.UpdatedOn);
    Assert.True(dictionary.Version > 1);

    Assert.Equal(3, dictionary.Entries.Count());
    Assert.Equal(3, dictionary.EntryCount);
    Assert.Contains(dictionary.Entries, customAttribute => customAttribute.Key == "Red" && customAttribute.Value == "Rouge");
    Assert.Contains(dictionary.Entries, customAttribute => customAttribute.Key == "Blue" && customAttribute.Value == "Cyan");
    Assert.Contains(dictionary.Entries, customAttribute => customAttribute.Key == "Yellow" && customAttribute.Value == "Jaune");
  }
}
