using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.SendGrid;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Senders.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadDefaultSenderQueryTests : IntegrationTests
{
  private readonly ISenderRepository _senderRepository;

  private readonly SenderAggregate _sender;

  public ReadDefaultSenderQueryTests() : base()
  {
    _senderRepository = ServiceProvider.GetRequiredService<ISenderRepository>();

    EmailUnit email = new(Faker.Internet.Email(), isVerified: false);
    ReadOnlySendGridSettings settings = new(SendGridHelper.GenerateApiKey());
    _sender = new(email, settings);
    _sender.SetDefault();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [PortalDb.Senders.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    await _senderRepository.SaveAsync(_sender);
  }

  [Fact(DisplayName = "It should return null when the sender cannot be found.")]
  public async Task It_should_return_null_when_the_sender_cannot_be_found()
  {
    SetRealm();

    ReadDefaultSenderQuery query = new();
    Sender? sender = await ActivityPipeline.ExecuteAsync(query);
    Assert.Null(sender);
  }

  [Fact(DisplayName = "It should return the sender found by ID.")]
  public async Task It_should_return_the_sender_found_by_Id()
  {
    ReadDefaultSenderQuery query = new();
    Sender? sender = await ActivityPipeline.ExecuteAsync(query);
    Assert.NotNull(sender);
    Assert.Equal(_sender.Id.ToGuid(), sender.Id);
  }
}
