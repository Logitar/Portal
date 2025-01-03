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
public class UpdateDictionaryCommandTests : IntegrationTests
{
  private readonly IDictionaryRepository _dictionaryRepository;

  private readonly Dictionary _dictionary;

  public UpdateDictionaryCommandTests() : base()
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

  [Fact(DisplayName = "It should return null when the dictionary cannot be found.")]
  public async Task It_should_return_null_when_the_dictionary_cannot_be_found()
  {
    UpdateDictionaryPayload payload = new();
    UpdateDictionaryCommand command = new(Guid.NewGuid(), payload);
    DictionaryModel? dictionary = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(dictionary);
  }

  [Fact(DisplayName = "It should return null when the dictionary is in another tenant.")]
  public async Task It_should_return_null_when_the_dictionary_is_in_another_tenant()
  {
    SetRealm();

    UpdateDictionaryPayload payload = new();
    UpdateDictionaryCommand command = new(_dictionary.EntityId.ToGuid(), payload);
    DictionaryModel? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw DictionaryAlreadyExistsException when the dictionary already exists.")]
  public async Task It_should_throw_DictionaryAlreadyExistsException_when_the_dictionary_already_exists()
  {
    Dictionary dictionary = new(new Locale("fr-CA"));
    await _dictionaryRepository.SaveAsync(dictionary);

    UpdateDictionaryPayload payload = new()
    {
      Locale = Faker.Locale
    };
    UpdateDictionaryCommand command = new(dictionary.EntityId.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<DictionaryAlreadyExistsException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Null(exception.TenantId);
    Assert.Equal(payload.Locale, exception.Locale);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateDictionaryPayload payload = new()
    {
      Locale = " Hello World! "
    };
    UpdateDictionaryCommand command = new(Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("Locale", exception.Errors.Single().PropertyName);
  }

  [Fact(DisplayName = "It should update an existing dictionary.")]
  public async Task It_should_update_an_existing_dictionary()
  {
    _dictionary.SetEntry(new Identifier("bleu"), "blue");
    _dictionary.SetEntry(new Identifier("rouge"), "red");
    _dictionary.SetEntry(new Identifier("vert"), "green");
    _dictionary.Update();
    await _dictionaryRepository.SaveAsync(_dictionary);

    UpdateDictionaryPayload payload = new()
    {
      Locale = "en-CA"
    };
    payload.Entries.Add(new("jaune", "yellow"));
    payload.Entries.Add(new("rouge", "scarlet"));
    payload.Entries.Add(new("vert", value: null));
    UpdateDictionaryCommand command = new(_dictionary.EntityId.ToGuid(), payload);
    DictionaryModel? dictionary = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(dictionary);

    Assert.Equal(payload.Locale, dictionary.Locale.Code);
    Assert.Null(dictionary.Realm);

    Assert.Equal(3, dictionary.EntryCount);
    Assert.Equal(3, dictionary.Entries.Count);
    Assert.Contains(dictionary.Entries, c => c.Key == "bleu" && c.Value == "blue");
    Assert.Contains(dictionary.Entries, c => c.Key == "rouge" && c.Value == "scarlet");
    Assert.Contains(dictionary.Entries, c => c.Key == "jaune" && c.Value == "yellow");
  }
}
