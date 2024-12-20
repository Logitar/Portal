﻿using Logitar.Data;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Senders;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.SendGrid;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
    Message? message = await ActivityPipeline.ExecuteAsync(query);
    Assert.Null(message);
  }

  [Fact(DisplayName = "It should return the message when it is found.")]
  public async Task It_should_return_the_message_when_it_is_found()
  {
    EmailUnit email = new(Faker.Internet.Email(), isVerified: false);
    ReadOnlySendGridSettings settings = new(SendGridHelper.GenerateApiKey());
    SenderAggregate sender = new(email, settings, TenantId);
    await _senderRepository.SaveAsync(sender);

    UniqueKeyUnit uniqueKey = new("PasswordRecovery");
    SubjectUnit subject = new("Reset your password");
    ContentUnit content = ContentUnit.PlainText("Hello World!");
    TemplateAggregate template = new(uniqueKey, subject, content, TenantId);
    await _templateRepository.SaveAsync(template);

    RecipientUnit[] recipients = [new RecipientUnit(RecipientType.To, Faker.Person.Email, Faker.Person.FullName)];
    MessageAggregate message = new(subject, content, recipients, sender, template, tenantId: TenantId);
    await _messageRepository.SaveAsync(message);

    SetRealm();

    ReadMessageQuery query = new(message.Id.ToGuid());
    Message? result = await ActivityPipeline.ExecuteAsync(query);
    Assert.NotNull(result);
    Assert.Equal(message.Id.ToGuid(), result.Id);
  }
}
