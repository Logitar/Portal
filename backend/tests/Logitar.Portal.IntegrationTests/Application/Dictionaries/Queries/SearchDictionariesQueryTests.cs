﻿using Logitar.Data;
using Logitar.Identity.Core;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortalDb = Logitar.Portal.EntityFrameworkCore.Relational.PortalDb;

namespace Logitar.Portal.Application.Dictionaries.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchDictionariesQueryTests : IntegrationTests
{
  private readonly IDictionaryRepository _dictionaryRepository;

  private readonly Dictionary _dictionary;

  public SearchDictionariesQueryTests() : base()
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

  [Fact(DisplayName = "It should return empty results when no dictionary did match.")]
  public async Task It_should_return_empty_results_when_no_dictionary_did_match()
  {
    SearchDictionariesPayload payload = new();
    payload.Search.Terms.Add(new SearchTerm("%test%"));
    SearchDictionariesQuery query = new(payload);
    SearchResults<DictionaryModel> results = await ActivityPipeline.ExecuteAsync(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    Dictionary notMatching = new(new Locale(Faker.Locale), actorId: null, DictionaryId.NewId(TenantId));
    Dictionary notInIds = new(new Locale("fr"), actorId: null, DictionaryId.NewId(TenantId));
    Dictionary empty = new(new Locale("fr-FR"), actorId: null, DictionaryId.NewId(TenantId));
    Dictionary notEmpty = new(new Locale("fr-CA"), actorId: null, DictionaryId.NewId(TenantId));
    notEmpty.SetEntry(new Identifier("Poutine"), "Poutine");
    notEmpty.Update();
    await _dictionaryRepository.SaveAsync([notMatching, notInIds, empty, notEmpty]);

    SetRealm();

    SearchDictionariesPayload payload = new()
    {
      Skip = 1,
      Limit = 1
    };
    IEnumerable<Guid> dictionaryIds = (await _dictionaryRepository.LoadAsync()).Select(dictionary => dictionary.EntityId.ToGuid());
    payload.Ids.AddRange(dictionaryIds);
    payload.Ids.Add(Guid.NewGuid());
    payload.Ids.Remove(notInIds.EntityId.ToGuid());
    payload.Search.Terms.Add(new SearchTerm("fr"));
    payload.Search.Terms.Add(new SearchTerm("fr___"));
    payload.Search.Operator = SearchOperator.Or;
    payload.Sort.Add(new DictionarySortOption(DictionarySort.EntryCount, isDescending: false));
    SearchDictionariesQuery query = new(payload);
    SearchResults<DictionaryModel> results = await ActivityPipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    DictionaryModel dictionary = Assert.Single(results.Items);
    Assert.Equal(notEmpty.EntityId.ToGuid(), dictionary.Id);
  }
}
