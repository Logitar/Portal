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
public class DeleteDictionaryCommandTests : IntegrationTests
{
  private readonly IDictionaryRepository _dictionaryRepository;

  private readonly DictionaryAggregate _dictionary;

  public DeleteDictionaryCommandTests() : base()
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

  [Fact(DisplayName = "It should delete an existing dictionary.")]
  public async Task It_should_delete_an_existing_dictionary()
  {
    DeleteDictionaryCommand command = new(_dictionary.Id.AggregateId.ToGuid());
    Dictionary? dictionary = await Mediator.Send(command);
    Assert.NotNull(dictionary);
    Assert.Equal(command.Id, dictionary.Id);
  }

  [Fact(DisplayName = "It should return null when the dictionary cannot be found.")]
  public async Task It_should_return_null_when_the_dictionary_cannot_be_found()
  {
    DeleteDictionaryCommand command = new(Guid.NewGuid());
    Dictionary? dictionary = await Mediator.Send(command);
    Assert.Null(dictionary);
  }

  [Fact(DisplayName = "It should return null when the dictionary is in another tenant.")]
  public async Task It_should_return_null_when_the_dictionary_is_in_another_tenant()
  {
    SetRealm();

    DeleteDictionaryCommand command = new(_dictionary.Id.AggregateId.ToGuid());
    Dictionary? result = await Mediator.Send(command);
    Assert.Null(result);
  }
}
