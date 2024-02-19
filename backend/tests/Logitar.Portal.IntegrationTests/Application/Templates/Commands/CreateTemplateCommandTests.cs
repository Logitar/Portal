using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
      ICommand command = SqlServerDeleteBuilder.From(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should create a new template.")]
  public async Task It_should_create_a_new_template()
  {
    Content content = Content.PlainText("Hello World!");
    CreateTemplatePayload payload = new("PasswordRecovery", "Reset your password", content)
    {
      DisplayName = " Reset Password ",
      Description = "This is the password recovery message template."
    };
    CreateTemplateCommand command = new(payload);
    Template template = await Mediator.Send(command);

    Assert.Equal(payload.UniqueKey, template.UniqueKey);
    Assert.Equal(payload.DisplayName.Trim(), template.DisplayName);
    Assert.Equal(payload.Description, template.Description);
    Assert.Equal(payload.Subject, template.Subject);
    Assert.Equal(payload.Content, template.Content);
    Assert.Null(template.Realm);

    SetRealm();
    Template other = await Mediator.Send(command);
    Assert.Same(Realm, other.Realm);
  }

  [Fact(DisplayName = "It should throw UniqueKeyAlreadyUsedException when the unique name is already used.")]
  public async Task It_should_throw_UniqueKeyAlreadyUsedException_when_the_unique_name_is_already_used()
  {
    SetRealm();

    ContentUnit content = ContentUnit.PlainText("Hello World!");
    TemplateAggregate template = new(new UniqueKeyUnit("PasswordRecovery"), new SubjectUnit("Reset your password"), content, TenantId);
    await _templateRepository.SaveAsync(template);

    CreateTemplatePayload payload = new(template.UniqueKey.Value, template.Subject.Value, new Content(content));
    CreateTemplateCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UniqueKeyAlreadyUsedException>(async () => await Mediator.Send(command));
    Assert.Equal(TenantId, exception.TenantId);
    Assert.Equal(template.UniqueKey, exception.UniqueKey);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    Content content = Content.PlainText("Hello World!");
    CreateTemplatePayload payload = new(uniqueKey: "", "Reset your password", content);
    CreateTemplateCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.Equal("UniqueKey", exception.Errors.Single().PropertyName);
  }
}
