using Logitar.Data;
using Logitar.Identity.Core;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortalDb = Logitar.Portal.EntityFrameworkCore.Relational.PortalDb;

namespace Logitar.Portal.Application.Templates.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateTemplateCommandTests : IntegrationTests
{
  private readonly ITemplateRepository _templateRepository;

  private readonly Template _template;

  public UpdateTemplateCommandTests() : base()
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

  [Fact(DisplayName = "It should return null when the template cannot be found.")]
  public async Task It_should_return_null_when_the_template_cannot_be_found()
  {
    UpdateTemplatePayload payload = new();
    UpdateTemplateCommand command = new(Guid.NewGuid(), payload);
    TemplateModel? template = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(template);
  }

  [Fact(DisplayName = "It should return null when the template is in another tenant.")]
  public async Task It_should_return_null_when_the_template_is_in_another_tenant()
  {
    SetRealm();

    UpdateTemplatePayload payload = new();
    UpdateTemplateCommand command = new(_template.EntityId.ToGuid(), payload);
    TemplateModel? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw UniqueKeyAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueKeyAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    Template template = new(new Identifier("ConfirmAccount"), new Subject("Confirm your account"), _template.Content);
    await _templateRepository.SaveAsync(template);

    UpdateTemplatePayload payload = new()
    {
      UniqueKey = "PaSSWoRDReCoVeRy"
    };
    UpdateTemplateCommand command = new(template.EntityId.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<UniqueKeyAlreadyUsedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Null(exception.TenantId);
    Assert.Equal(payload.UniqueKey, exception.UniqueKey);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateTemplatePayload payload = new()
    {
      UniqueKey = "/!\\"
    };
    UpdateTemplateCommand command = new(Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("UniqueKey", exception.Errors.Single().PropertyName);
  }

  [Fact(DisplayName = "It should update an existing template.")]
  public async Task It_should_update_an_existing_template()
  {
    UpdateTemplatePayload payload = new()
    {
      DisplayName = new ChangeModel<string>("  ConfirmAccount  "),
      Description = new ChangeModel<string>("  ")
    };
    UpdateTemplateCommand command = new(_template.EntityId.ToGuid(), payload);
    TemplateModel? template = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(template);

    Assert.Equal(_template.UniqueKey.Value, template.UniqueKey);
    Assert.NotNull(payload.DisplayName.Value);
    Assert.Equal(payload.DisplayName.Value.Trim(), template.DisplayName);
    Assert.Null(template.Description);
    Assert.Null(template.Realm);
  }
}
