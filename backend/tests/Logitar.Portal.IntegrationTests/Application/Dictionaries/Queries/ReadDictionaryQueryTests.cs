using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Dictionaries.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadDictionaryQueryTests : IntegrationTests
{
  private readonly IDictionaryRepository _dictionaryRepository;

  private readonly DictionaryAggregate _dictionary;

  public ReadDictionaryQueryTests() : base()
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

  [Fact(DisplayName = "It should return null when the dictionary cannot be found.")]
  public async Task It_should_return_null_when_the_dictionary_cannot_be_found()
  {
    SetRealm();

    ReadDictionaryQuery query = new(_dictionary.Id.ToGuid(), Locale: null);
    Dictionary? dictionary = await Mediator.Send(query);
    Assert.Null(dictionary);
  }

  [Fact(DisplayName = "It should return the dictionary found by ID.")]
  public async Task It_should_return_the_dictionary_found_by_Id()
  {
    ReadDictionaryQuery query = new(_dictionary.Id.ToGuid(), _dictionary.Locale.Code);
    Dictionary? dictionary = await Mediator.Send(query);
    Assert.NotNull(dictionary);
    Assert.Equal(_dictionary.Id.ToGuid(), dictionary.Id);
  }

  [Fact(DisplayName = "It should return the dictionary found by locale.")]
  public async Task It_should_return_the_dictionary_found_by_locale()
  {
    ReadDictionaryQuery query = new(Id: null, _dictionary.Locale.Code);
    Dictionary? dictionary = await Mediator.Send(query);
    Assert.NotNull(dictionary);
    Assert.Equal(_dictionary.Id.ToGuid(), dictionary.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when there are too many results.")]
  public async Task It_should_throw_TooManyResultsException_when_there_are_too_many_results()
  {
    DictionaryAggregate dictionary = new(new LocaleUnit("fr"));
    await _dictionaryRepository.SaveAsync(dictionary);

    ReadDictionaryQuery query = new(_dictionary.Id.ToGuid(), "  FR  ");
    var exception = await Assert.ThrowsAsync<TooManyResultsException<Dictionary>>(async () => await Mediator.Send(query));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
