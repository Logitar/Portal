using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Portal.Contracts;
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

  private readonly Template _template;

  public ReadTemplateQueryTests() : base()
  {
    _templateRepository = ServiceProvider.GetRequiredService<ITemplateRepository>();

    UniqueKey uniqueKey = new("PasswordRecovery");
    Subject subject = new("Reset your password");
    Content content = Content.PlainText("Hello World!");
    _template = new(uniqueKey, subject, content);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [PortalDb.Templates.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    await _templateRepository.SaveAsync(_template);
  }

  [Fact(DisplayName = "It should return null when the template cannot be found.")]
  public async Task It_should_return_null_when_the_template_cannot_be_found()
  {
    SetRealm();

    ReadTemplateQuery query = new(_template.Id.ToGuid(), UniqueKey: null);
    TemplateModel? template = await ActivityPipeline.ExecuteAsync(query);
    Assert.Null(template);
  }

  [Fact(DisplayName = "It should return the template found by ID.")]
  public async Task It_should_return_the_template_found_by_Id()
  {
    ReadTemplateQuery query = new(_template.Id.ToGuid(), _template.UniqueKey.Value);
    TemplateModel? template = await ActivityPipeline.ExecuteAsync(query);
    Assert.NotNull(template);
    Assert.Equal(_template.Id.ToGuid(), template.Id);
  }

  [Fact(DisplayName = "It should return the template found by unique key.")]
  public async Task It_should_return_the_template_found_by_unique_key()
  {
    ReadTemplateQuery query = new(Id: null, _template.UniqueKey.Value);
    TemplateModel? template = await ActivityPipeline.ExecuteAsync(query);
    Assert.NotNull(template);
    Assert.Equal(_template.Id.ToGuid(), template.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when there are too many results.")]
  public async Task It_should_throw_TooManyResultsException_when_there_are_too_many_results()
  {
    UniqueKey uniqueKey = new("ConfirmAccount");
    Subject subject = new("Confirm your account");
    Template template = new(uniqueKey, subject, _template.Content);
    await _templateRepository.SaveAsync(template);

    ReadTemplateQuery query = new(_template.Id.ToGuid(), "  CoNFiRMaCCouNT  ");
    var exception = await Assert.ThrowsAsync<TooManyResultsException<TemplateModel>>(async () => await ActivityPipeline.ExecuteAsync(query));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
