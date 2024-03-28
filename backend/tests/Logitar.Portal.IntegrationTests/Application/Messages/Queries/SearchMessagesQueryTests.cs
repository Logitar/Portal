using Logitar.Data;
using Logitar.Identity.Domain.Users;
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
    SearchResults<Message> results = await ActivityPipeline.ExecuteAsync(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SetRealm();

    EmailUnit email = new(Faker.Internet.Email(), isVerified: false);
    ReadOnlySendGridSettings settings = new(SendGridHelper.GenerateApiKey());
    SenderAggregate sender = new(email, settings, TenantId);
    await _senderRepository.SaveAsync(sender);

    SubjectUnit subject = new("Reset your password");
    ContentUnit content = ContentUnit.PlainText("Hello World!");
    TemplateAggregate template = new(new UniqueKeyUnit("PasswordRecovery"), subject, content, TenantId);
    SubjectUnit otherSubject = new("Confirm your account");
    TemplateAggregate otherTemplate = new(new UniqueKeyUnit("AccountConfirmation"), otherSubject, content, TenantId);
    await _templateRepository.SaveAsync([template, otherTemplate]);

    RecipientUnit[] recipients = [new RecipientUnit(RecipientType.To, Faker.Person.Email, Faker.Person.FullName)];

    MessageAggregate notMatching = new(otherSubject, content, recipients, sender, template, tenantId: TenantId);
    MessageAggregate notInIds = new(subject, content, recipients, sender, template, tenantId: TenantId);
    MessageAggregate notTemplate = new(subject, content, recipients, sender, otherTemplate, tenantId: TenantId);
    MessageAggregate demo = new(subject, content, recipients, sender, template, isDemo: true, tenantId: TenantId);
    MessageAggregate failed = new(subject, content, recipients, sender, template, tenantId: TenantId);
    failed.Fail();

    MessageAggregate message1 = new(subject, content, recipients, sender, template, tenantId: TenantId);

    RecipientUnit[] moreRecipients =
    [
      recipients.Single(),
      new RecipientUnit(RecipientType.Bcc, Faker.Internet.Email(), Faker.Name.FullName())
    ];
    MessageAggregate message2 = new(subject, content, moreRecipients, sender, template, tenantId: TenantId);

    await _messageRepository.SaveAsync([notMatching, notInIds, notTemplate, demo, failed, message1, message2]);

    SearchMessagesPayload payload = new()
    {
      TemplateId = template.Id.ToGuid(),
      IsDemo = false,
      Status = MessageStatus.Unsent,
      Skip = 1,
      Limit = 1
    };
    IEnumerable<Guid> messageIds = (await _messageRepository.LoadAsync()).Select(message => message.Id.ToGuid());
    payload.Ids.AddRange(messageIds);
    payload.Ids.Add(Guid.NewGuid());
    payload.Ids.Remove(notInIds.Id.ToGuid());
    payload.Search.Terms.Add(new SearchTerm("Reset%"));
    payload.Sort.Add(new MessageSortOption(MessageSort.RecipientCount, isDescending: false));
    SearchMessagesQuery query = new(payload);
    SearchResults<Message> results = await ActivityPipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    Message message = Assert.Single(results.Items);
    Assert.Equal(message2.Id.ToGuid(), message.Id);
  }
}
