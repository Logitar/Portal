using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.SendGrid;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Senders.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class SetDefaultSenderCommandTests : IntegrationTests
{
  private readonly ISenderRepository _senderRepository;

  private readonly SenderAggregate _sender;

  public SetDefaultSenderCommandTests() : base()
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
    SetDefaultSenderCommand command = new(Guid.NewGuid());
    Sender? sender = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(sender);
  }

  [Fact(DisplayName = "It should return null when the sender is in another tenant.")]
  public async Task It_should_return_null_when_the_sender_is_in_another_tenant()
  {
    SetRealm();

    SetDefaultSenderCommand command = new(_sender.Id.ToGuid());
    Sender? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should set the default sender.")]
  public async Task It_should_set_the_default_sender()
  {
    SenderAggregate sender = new(_sender.Email, _sender.Settings);
    await _senderRepository.SaveAsync(sender);

    SetDefaultSenderCommand command = new(sender.Id.ToGuid());
    Sender? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(result);
    Assert.Equal(command.Id, result.Id);
    Assert.True(result.IsDefault);

    SenderAggregate? oldDefault = await _senderRepository.LoadAsync(_sender.Id, version: null);
    Assert.NotNull(oldDefault);
    Assert.False(oldDefault.IsDefault);
  }
}
