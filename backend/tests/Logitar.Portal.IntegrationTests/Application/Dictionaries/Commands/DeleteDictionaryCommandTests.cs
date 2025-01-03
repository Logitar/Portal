﻿using Logitar.Data;
using Logitar.Identity.Core;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortalDb = Logitar.Portal.EntityFrameworkCore.Relational.PortalDb;

namespace Logitar.Portal.Application.Dictionaries.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class DeleteDictionaryCommandTests : IntegrationTests
{
  private readonly IDictionaryRepository _dictionaryRepository;

  private readonly Dictionary _dictionary;

  public DeleteDictionaryCommandTests() : base()
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

  [Fact(DisplayName = "It should delete an existing dictionary.")]
  public async Task It_should_delete_an_existing_dictionary()
  {
    DeleteDictionaryCommand command = new(_dictionary.EntityId.ToGuid());
    DictionaryModel? dictionary = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(dictionary);
    Assert.Equal(command.Id, dictionary.Id);
  }

  [Fact(DisplayName = "It should return null when the dictionary cannot be found.")]
  public async Task It_should_return_null_when_the_dictionary_cannot_be_found()
  {
    DeleteDictionaryCommand command = new(Guid.NewGuid());
    DictionaryModel? dictionary = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(dictionary);
  }

  [Fact(DisplayName = "It should return null when the dictionary is in another tenant.")]
  public async Task It_should_return_null_when_the_dictionary_is_in_another_tenant()
  {
    SetRealm();

    DeleteDictionaryCommand command = new(_dictionary.EntityId.ToGuid());
    DictionaryModel? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(result);
  }
}
