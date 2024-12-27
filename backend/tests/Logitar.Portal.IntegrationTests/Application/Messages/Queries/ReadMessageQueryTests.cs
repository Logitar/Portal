using Bogus;
using Logitar.Data;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Application.Senders;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.SendGrid;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortalDb = Logitar.Portal.EntityFrameworkCore.Relational.PortalDb;

namespace Logitar.Portal.Application.Messages.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadMessageQueryTests : IntegrationTests
{
  private readonly IMessageRepository _messageRepository;
  private readonly ISenderRepository _senderRepository;
  private readonly ITemplateRepository _templateRepository;

  public ReadMessageQueryTests() : base()
  {
    _messageRepository = ServiceProvider.GetRequiredService<IMessageRepository>();
    _senderRepository = ServiceProvider.GetRequiredService<ISenderRepository>();
    _templateRepository = ServiceProvider.GetRequiredService<ITemplateRepository>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [PortalDb.Messages.Table, PortalDb.Senders.Table, PortalDb.Templates.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should return null when the message cannot be found.")]
  public async Task It_should_return_null_when_the_message_cannot_be_found()
  {
    ReadMessageQuery query = new(Guid.NewGuid());
    MessageModel? message = await ActivityPipeline.ExecuteAsync(query);
    Assert.Null(message);
  }

  [Fact(DisplayName = "It should return the message when it is found.")]
  public async Task It_should_return_the_message_when_it_is_found()
  {
    Email email = new(Faker.Internet.Email(), isVerified: false);
    ReadOnlySendGridSettings settings = new(SendGridHelper.GenerateApiKey());
    Sender sender = new(email, settings, actorId: null, SenderId.NewId(TenantId));
    await _senderRepository.SaveAsync(sender);

    Identifier uniqueKey = new("PasswordRecovery");
    Subject subject = new("Reset your password");
    Content content = Content.PlainText("Hello World!");
    Template template = new(uniqueKey, subject, content, actorId: null, TemplateId.NewId(TenantId));
    await _templateRepository.SaveAsync(template);

    Recipient[] recipients = [new Recipient(RecipientType.To, Faker.Person.Email, Faker.Person.FullName)];
    Message message = new(subject, content, recipients, sender, template, tenantId: TenantId);
    await _messageRepository.SaveAsync(message);

    SetRealm();

    ReadMessageQuery query = new(message.EntityId.ToGuid());
    MessageModel? result = await ActivityPipeline.ExecuteAsync(query);
    Assert.NotNull(result);
    Assert.Equal(message.EntityId.ToGuid(), result.Id);
  }
}
