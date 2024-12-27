using Logitar.Data;
using Logitar.Identity.Core;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortalDb = Logitar.Portal.EntityFrameworkCore.Relational.PortalDb;

namespace Logitar.Portal.Application.Templates.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class DeleteTemplateCommandTests : IntegrationTests
{
  private readonly ITemplateRepository _templateRepository;

  private readonly Template _template;

  public DeleteTemplateCommandTests() : base()
  {
    _templateRepository = ServiceProvider.GetRequiredService<ITemplateRepository>();

    Identifier uniqueKey = new("PasswordRecovery");
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

  [Fact(DisplayName = "It should delete an existing template.")]
  public async Task It_should_delete_an_existing_template()
  {
    DeleteTemplateCommand command = new(_template.EntityId.ToGuid());
    TemplateModel? template = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(template);
    Assert.Equal(command.Id, template.Id);
  }

  [Fact(DisplayName = "It should return null when the template cannot be found.")]
  public async Task It_should_return_null_when_the_template_cannot_be_found()
  {
    DeleteTemplateCommand command = new(Guid.NewGuid());
    TemplateModel? template = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(template);
  }

  [Fact(DisplayName = "It should return null when the template is in another tenant.")]
  public async Task It_should_return_null_when_the_template_is_in_another_tenant()
  {
    SetRealm();

    DeleteTemplateCommand command = new(_template.EntityId.ToGuid());
    TemplateModel? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(result);
  }
}
