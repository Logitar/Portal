using Logitar.Data;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortalDb = Logitar.Portal.EntityFrameworkCore.Relational.PortalDb;

namespace Logitar.Portal.Application.Templates.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceTemplateCommandTests : IntegrationTests
{
  private readonly ITemplateRepository _templateRepository;

  private readonly Template _template;

  public ReplaceTemplateCommandTests() : base()
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

  [Fact(DisplayName = "It should replace an existing template.")]
  public async Task It_should_replace_an_existing_template()
  {
    _template.DisplayName = new DisplayName("Reset Password");
    _template.Update();
    await _templateRepository.SaveAsync(_template);
    long version = _template.Version;

    DisplayName displayName = new("Password Recovery");
    _template.DisplayName = displayName;
    _template.Update();
    await _templateRepository.SaveAsync(_template);

    ReplaceTemplatePayload payload = new("PasswordRecovery", "Reset your password", new ContentModel(_template.Content))
    {
      DisplayName = "  Reset Password  ",
      Description = " This is the password recovery message template. "
    };
    ReplaceTemplateCommand command = new(_template.Id.ToGuid(), payload, version);
    TemplateModel? template = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(template);

    Assert.Equal(payload.UniqueKey, template.UniqueKey);
    Assert.Equal(displayName.Value, template.DisplayName);
    Assert.Equal(payload.Description.Trim(), template.Description);
    Assert.Equal(payload.Subject, template.Subject);
    Assert.Equal(payload.Content, template.Content);
  }

  [Fact(DisplayName = "It should return null when the template cannot be found.")]
  public async Task It_should_return_null_when_the_template_cannot_be_found()
  {
    ReplaceTemplatePayload payload = new("PasswordRecovery", "Reset your password", new ContentModel(_template.Content));
    ReplaceTemplateCommand command = new(Guid.NewGuid(), payload, Version: null);
    TemplateModel? template = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(template);
  }

  [Fact(DisplayName = "It should return null when the template is in another tenant.")]
  public async Task It_should_return_null_when_the_template_is_in_another_tenant()
  {
    SetRealm();

    ReplaceTemplatePayload payload = new("PasswordRecovery", "Reset your password", new ContentModel(_template.Content));
    ReplaceTemplateCommand command = new(_template.Id.ToGuid(), payload, Version: null);
    TemplateModel? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw UniqueKeyAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueKeyAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    Template template = new(new UniqueKey("PasswordRecovery_OLD"), _template.Subject, _template.Content);
    await _templateRepository.SaveAsync(template);

    ReplaceTemplatePayload payload = new("PaSSWoRDReCoVeRy", _template.Subject.Value, new ContentModel(_template.Content));
    ReplaceTemplateCommand command = new(template.Id.ToGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<UniqueKeyAlreadyUsedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Null(exception.TenantId);
    Assert.Equal(payload.UniqueKey, exception.UniqueKey.Value);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceTemplatePayload payload = new("/!\\", _template.Subject.Value, new ContentModel(_template.Content));
    ReplaceTemplateCommand command = new(Guid.NewGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("UniqueKey", exception.Errors.Single().PropertyName);
  }
}
