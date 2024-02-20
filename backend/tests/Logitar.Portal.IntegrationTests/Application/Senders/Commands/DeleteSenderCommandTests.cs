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
public class DeleteSenderCommandTests : IntegrationTests
{
  private readonly ISenderRepository _senderRepository;

  private readonly SenderAggregate _sender;

  public DeleteSenderCommandTests() : base()
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
      ICommand command = SqlServerDeleteBuilder.From(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }

    await _senderRepository.SaveAsync(_sender);
  }

  [Fact(DisplayName = "It should delete an existing sender.")]
  public async Task It_should_delete_an_existing_sender()
  {
    DeleteSenderCommand command = new(_sender.Id.AggregateId.ToGuid());
    Sender? sender = await Mediator.Send(command);
    Assert.NotNull(sender);
    Assert.Equal(command.Id, sender.Id);
  }

  [Fact(DisplayName = "It should return null when the sender cannot be found.")]
  public async Task It_should_return_null_when_the_sender_cannot_be_found()
  {
    DeleteSenderCommand command = new(Guid.NewGuid());
    Sender? sender = await Mediator.Send(command);
    Assert.Null(sender);
  }

  [Fact(DisplayName = "It should return null when the sender is in another tenant.")]
  public async Task It_should_return_null_when_the_sender_is_in_another_tenant()
  {
    SetRealm();

    DeleteSenderCommand command = new(_sender.Id.AggregateId.ToGuid());
    Sender? result = await Mediator.Send(command);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw CannotDeleteDefaultSenderException when the default sender is not unique in its realm.")]
  public async Task It_should_throw_CannotDeleteDefaultSenderException_when_the_default_sender_is_not_unique_in_its_realm()
  {
    SenderAggregate other = new(_sender.Email, _sender.Settings);
    await _senderRepository.SaveAsync(other);

    DeleteSenderCommand command = new(_sender.Id.AggregateId.ToGuid());
    var exception = await Assert.ThrowsAsync<CannotDeleteDefaultSenderException>(async () => await Mediator.Send(command));
    Assert.Equal(_sender.Id, exception.SenderId);
  }
}
