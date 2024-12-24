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
public class CreateTemplateCommandTests : IntegrationTests
{
  private readonly ITemplateRepository _templateRepository;

  public CreateTemplateCommandTests() : base()
  {
    _templateRepository = ServiceProvider.GetRequiredService<ITemplateRepository>();
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
  }

  [Fact(DisplayName = "It should create a new template.")]
  public async Task It_should_create_a_new_template()
  {
    ContentModel content = ContentModel.PlainText("Hello World!");
    CreateTemplatePayload payload = new("PasswordRecovery", "Reset your password", content)
    {
      DisplayName = " Reset Password ",
      Description = "This is the password recovery message template."
    };
    CreateTemplateCommand command = new(payload);
    TemplateModel template = await ActivityPipeline.ExecuteAsync(command);

    Assert.Equal(payload.UniqueKey, template.UniqueKey);
    Assert.Equal(payload.DisplayName.Trim(), template.DisplayName);
    Assert.Equal(payload.Description, template.Description);
    Assert.Equal(payload.Subject, template.Subject);
    Assert.Equal(payload.Content, template.Content);
    Assert.Null(template.Realm);

    SetRealm();
    TemplateModel other = await ActivityPipeline.ExecuteAsync(command);
    Assert.Same(Realm, other.Realm);
  }

  [Fact(DisplayName = "It should throw UniqueKeyAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueKeyAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    SetRealm();

    Content content = Content.PlainText("Hello World!");
    Template template = new(new Identifier("PasswordRecovery"), new Subject("Reset your password"), content, id: TemplateId.NewId(TenantId));
    await _templateRepository.SaveAsync(template);

    CreateTemplatePayload payload = new(template.UniqueKey.Value, template.Subject.Value, new ContentModel(content));
    CreateTemplateCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UniqueKeyAlreadyUsedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(TenantId.Value, exception.TenantId);
    Assert.Equal(template.UniqueKey.Value, exception.UniqueKey);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ContentModel content = ContentModel.PlainText("Hello World!");
    CreateTemplatePayload payload = new(uniqueKey: "", "Reset your password", content);
    CreateTemplateCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("UniqueKey", exception.Errors.Single().PropertyName);
  }
}
