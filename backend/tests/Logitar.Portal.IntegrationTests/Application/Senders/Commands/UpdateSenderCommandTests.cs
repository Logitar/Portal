using Logitar.Data;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.Mailgun;
using Logitar.Portal.Domain.Senders.SendGrid;
using Logitar.Portal.Domain.Senders.Twilio;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortalDb = Logitar.Portal.EntityFrameworkCore.Relational.PortalDb;

namespace Logitar.Portal.Application.Senders.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateSenderCommandTests : IntegrationTests
{
  private readonly ISenderRepository _senderRepository;

  private readonly Sender _mailgun;
  private readonly Sender _sendGrid;
  private readonly Sender _twilio;

  public UpdateSenderCommandTests() : base()
  {
    _senderRepository = ServiceProvider.GetRequiredService<ISenderRepository>();

    Email email = new(Faker.Internet.Email(), isVerified: false);
    _mailgun = new(email, new ReadOnlyMailgunSettings(MailgunHelper.GenerateApiKey(), Faker.Internet.DomainName()));
    _sendGrid = new(email, new ReadOnlySendGridSettings(SendGridHelper.GenerateApiKey()));
    _sendGrid.SetDefault();

    Phone phone = new("+15148454636", countryCode: null, extension: null, isVerified: false);
    _twilio = new(phone, new ReadOnlyTwilioSettings(TwilioHelper.GenerateAccountSid(), TwilioHelper.GenerateAuthenticationToken()))
    {
      Description = new Description("This is the SMS sender.")
    };
    _twilio.Update();
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

    await _senderRepository.SaveAsync([_sendGrid, _mailgun, _twilio]);
  }

  [Fact(DisplayName = "It should return null when the sender cannot be found.")]
  public async Task It_should_return_null_when_the_sender_cannot_be_found()
  {
    UpdateSenderPayload payload = new();
    UpdateSenderCommand command = new(Guid.NewGuid(), payload);
    SenderModel? sender = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(sender);
  }

  [Fact(DisplayName = "It should return null when the sender is in another tenant.")]
  public async Task It_should_return_null_when_the_sender_is_in_another_tenant()
  {
    SetRealm();

    UpdateSenderPayload payload = new();
    UpdateSenderCommand command = new(_sendGrid.EntityId.ToGuid(), payload);
    SenderModel? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateSenderPayload payload = new()
    {
      EmailAddress = "aa@@bb..cc"
    };
    UpdateSenderCommand command = new(_sendGrid.EntityId.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("EmailAddress", exception.Errors.Single().PropertyName);
  }

  [Fact(DisplayName = "It should update an existing Mailgun sender.")]
  public async Task It_should_update_an_existing_Mailgun_sender()
  {
    UpdateSenderPayload payload = new()
    {
      DisplayName = new ChangeModel<string>("  Alternative Sender  "),
      Description = new ChangeModel<string>("  ")
    };
    UpdateSenderCommand command = new(_mailgun.EntityId.ToGuid(), payload);
    SenderModel? sender = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(sender);

    Assert.Equal(_mailgun.Email?.Address, sender.EmailAddress);
    Assert.NotNull(payload.DisplayName.Value);
    Assert.Equal(payload.DisplayName.Value.Trim(), sender.DisplayName);
    Assert.Null(sender.Description);
    Assert.Null(sender.Realm);
  }

  [Fact(DisplayName = "It should update an existing SendGrid sender.")]
  public async Task It_should_update_an_existing_SendGrid_sender()
  {
    UpdateSenderPayload payload = new()
    {
      DisplayName = new ChangeModel<string>("  Default Sender  "),
      Description = new ChangeModel<string>("  ")
    };
    UpdateSenderCommand command = new(_sendGrid.EntityId.ToGuid(), payload);
    SenderModel? sender = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(sender);

    Assert.Equal(_sendGrid.Email?.Address, sender.EmailAddress);
    Assert.NotNull(payload.DisplayName.Value);
    Assert.Equal(payload.DisplayName.Value.Trim(), sender.DisplayName);
    Assert.Null(sender.Description);
    Assert.Null(sender.Realm);
  }

  [Fact(DisplayName = "It should update an existing Twilio sender.")]
  public async Task It_should_update_an_existing_Twilio_sender()
  {
    UpdateSenderPayload payload = new()
    {
      PhoneNumber = "+15148422112"
    };
    UpdateSenderCommand command = new(_twilio.EntityId.ToGuid(), payload);
    SenderModel? sender = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(sender);

    Assert.Equal(payload.PhoneNumber, sender.PhoneNumber);
    Assert.NotNull(_twilio.Description);
    Assert.Equal(_twilio.Description.Value, sender.Description);
    Assert.Null(sender.Realm);
  }
}
