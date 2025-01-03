﻿using Logitar.Data;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Application.Senders;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Search;
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
public class SearchMessagesQueryTests : IntegrationTests
{
  private readonly IMessageRepository _messageRepository;
  private readonly ISenderRepository _senderRepository;
  private readonly ITemplateRepository _templateRepository;

  public SearchMessagesQueryTests() : base()
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

  [Fact(DisplayName = "It should return empty results when no message did match.")]
  public async Task It_should_return_empty_results_when_no_message_did_match()
  {
    SearchMessagesPayload payload = new();
    SearchMessagesQuery query = new(payload);
    SearchResults<MessageModel> results = await ActivityPipeline.ExecuteAsync(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SetRealm();

    Email email = new(Faker.Internet.Email(), isVerified: false);
    ReadOnlySendGridSettings settings = new(SendGridHelper.GenerateApiKey());
    Sender sender = new(email, settings, actorId: null, SenderId.NewId(TenantId));
    await _senderRepository.SaveAsync(sender);

    Subject subject = new("Reset your password");
    Content content = Content.PlainText("Hello World!");
    Template template = new(new Identifier("PasswordRecovery"), subject, content, actorId: null, TemplateId.NewId(TenantId));
    Subject otherSubject = new("Confirm your account");
    Template otherTemplate = new(new Identifier("AccountConfirmation"), otherSubject, content, actorId: null, TemplateId.NewId(TenantId));
    await _templateRepository.SaveAsync([template, otherTemplate]);

    Recipient[] recipients = [new Recipient(RecipientType.To, Faker.Person.Email, Faker.Person.FullName)];

    Message notMatching = new(otherSubject, content, recipients, sender, template, tenantId: TenantId);
    Message notInIds = new(subject, content, recipients, sender, template, tenantId: TenantId);
    Message notTemplate = new(subject, content, recipients, sender, otherTemplate, tenantId: TenantId);
    Message demo = new(subject, content, recipients, sender, template, isDemo: true, tenantId: TenantId);
    Message failed = new(subject, content, recipients, sender, template, tenantId: TenantId);
    failed.Fail();

    Message message1 = new(subject, content, recipients, sender, template, tenantId: TenantId);

    Recipient[] moreRecipients =
    [
      recipients.Single(),
      new Recipient(RecipientType.Bcc, Faker.Internet.Email(), Faker.Name.FullName())
    ];
    Message message2 = new(subject, content, moreRecipients, sender, template, tenantId: TenantId);

    await _messageRepository.SaveAsync([notMatching, notInIds, notTemplate, demo, failed, message1, message2]);

    SearchMessagesPayload payload = new()
    {
      TemplateId = template.EntityId.ToGuid(),
      IsDemo = false,
      Status = MessageStatus.Unsent,
      Skip = 1,
      Limit = 1
    };
    IEnumerable<Guid> messageIds = (await _messageRepository.LoadAsync()).Select(message => message.EntityId.ToGuid());
    payload.Ids.AddRange(messageIds);
    payload.Ids.Add(Guid.NewGuid());
    payload.Ids.Remove(notInIds.EntityId.ToGuid());
    payload.Search.Terms.Add(new SearchTerm("Reset%"));
    payload.Sort.Add(new MessageSortOption(MessageSort.RecipientCount, isDescending: false));
    SearchMessagesQuery query = new(payload);
    SearchResults<MessageModel> results = await ActivityPipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    MessageModel message = Assert.Single(results.Items);
    Assert.Equal(message2.EntityId.ToGuid(), message.Id);
  }
}
