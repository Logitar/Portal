using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Templates.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadTemplateQueryTests : IntegrationTests
{
  private readonly ITemplateRepository _templateRepository;

  private readonly TemplateAggregate _template;

  public ReadTemplateQueryTests() : base()
  {
    _templateRepository = ServiceProvider.GetRequiredService<ITemplateRepository>();

    UniqueKeyUnit uniqueKey = new("PasswordRecovery");
    SubjectUnit subject = new("Reset your password");
    ContentUnit content = ContentUnit.PlainText("Hello World!");
    _template = new(uniqueKey, subject, content);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [PortalDb.Templates.Table];
    foreach (TableId table in tables)
    {
      ICommand command = SqlServerDeleteBuilder.From(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    await _templateRepository.SaveAsync(_template);
  }

  [Fact(DisplayName = "It should return null when the template cannot be found.")]
  public async Task It_should_return_null_when_the_template_cannot_be_found()
  {
    SetRealm();

    ReadTemplateQuery query = new(_template.Id.ToGuid(), UniqueKey: null);
    Template? template = await Mediator.Send(query);
    Assert.Null(template);
  }

  [Fact(DisplayName = "It should return the template found by ID.")]
  public async Task It_should_return_the_template_found_by_Id()
  {
    ReadTemplateQuery query = new(_template.Id.ToGuid(), _template.UniqueKey.Value);
    Template? template = await Mediator.Send(query);
    Assert.NotNull(template);
    Assert.Equal(_template.Id.ToGuid(), template.Id);
  }

  [Fact(DisplayName = "It should return the template found by unique key.")]
  public async Task It_should_return_the_template_found_by_unique_key()
  {
    ReadTemplateQuery query = new(Id: null, _template.UniqueKey.Value);
    Template? template = await Mediator.Send(query);
    Assert.NotNull(template);
    Assert.Equal(_template.Id.ToGuid(), template.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when there are too many results.")]
  public async Task It_should_throw_TooManyResultsException_when_there_are_too_many_results()
  {
    UniqueKeyUnit uniqueKey = new("ConfirmAccount");
    SubjectUnit subject = new("Confirm your account");
    TemplateAggregate template = new(uniqueKey, subject, _template.Content);
    await _templateRepository.SaveAsync(template);

    ReadTemplateQuery query = new(_template.Id.ToGuid(), "  CoNFiRMaCCouNT  ");
    var exception = await Assert.ThrowsAsync<TooManyResultsException<Template>>(async () => await Mediator.Send(query));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
