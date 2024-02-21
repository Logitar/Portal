using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Identity.Contracts;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.SendGrid;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Senders.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateSenderCommandTests : IntegrationTests
{
  private readonly ISenderRepository _senderRepository;

  private readonly SenderAggregate _sender;

  public UpdateSenderCommandTests() : base()
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

  [Fact(DisplayName = "It should return null when the sender cannot be found.")]
  public async Task It_should_return_null_when_the_sender_cannot_be_found()
  {
    UpdateSenderPayload payload = new();
    UpdateSenderCommand command = new(Guid.NewGuid(), payload);
    Sender? sender = await Mediator.Send(command);
    Assert.Null(sender);
  }

  [Fact(DisplayName = "It should return null when the sender is in another tenant.")]
  public async Task It_should_return_null_when_the_sender_is_in_another_tenant()
  {
    SetRealm();

    UpdateSenderPayload payload = new();
    UpdateSenderCommand command = new(_sender.Id.ToGuid(), payload);
    Sender? result = await Mediator.Send(command);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateSenderPayload payload = new()
    {
      Email = new EmailPayload("aa@@bb..cc", isVerified: false)
    };
    UpdateSenderCommand command = new(Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.Equal("Email.Address", exception.Errors.Single().PropertyName);
  }

  [Fact(DisplayName = "It should update an existing sender.")]
  public async Task It_should_update_an_existing_sender()
  {
    UpdateSenderPayload payload = new()
    {
      DisplayName = new Modification<string>("  Default Sender  "),
      Description = new Modification<string>("  ")
    };
    UpdateSenderCommand command = new(_sender.Id.ToGuid(), payload);
    Sender? sender = await Mediator.Send(command);
    Assert.NotNull(sender);

    Assert.Equal(_sender.Email.Address, sender.Email.Address);
    Assert.NotNull(payload.DisplayName.Value);
    Assert.Equal(payload.DisplayName.Value.Trim(), sender.DisplayName);
    Assert.Null(sender.Description);
    Assert.Null(sender.Realm);
  }
}
