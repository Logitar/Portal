using Logitar.Data;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.Mailgun;
using Logitar.Portal.Domain.Senders.SendGrid;
using Logitar.Portal.Domain.Senders.Twilio;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Senders.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceSenderCommandTests : IntegrationTests
{
  private readonly ISenderRepository _senderRepository;

  private readonly Sender _mailgun;
  private readonly Sender _sendGrid;
  private readonly Sender _twilio;

  public ReplaceSenderCommandTests() : base()
  {
    _senderRepository = ServiceProvider.GetRequiredService<ISenderRepository>();

    EmailUnit email = new(Faker.Internet.Email(), isVerified: false);
    _mailgun = new(email, new ReadOnlyMailgunSettings(MailgunHelper.GenerateApiKey(), Faker.Internet.DomainName()));
    _sendGrid = new(email, new ReadOnlySendGridSettings(SendGridHelper.GenerateApiKey()));
    _sendGrid.SetDefault();

    PhoneUnit phone = new("+15148454636", countryCode: null, extension: null, isVerified: false);
    _twilio = new(phone, new ReadOnlyTwilioSettings(TwilioHelper.GenerateAccountSid(), TwilioHelper.GenerateAuthenticationToken()));
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

    await _senderRepository.SaveAsync([_sendGrid, _mailgun]);
  }

  [Fact(DisplayName = "It should replace a Mailgun sender.")]
  public async Task It_should_replace_a_Mailgun_sender()
  {
    _mailgun.DisplayName = new DisplayNameUnit("Logitar");
    _mailgun.Update();
    await _senderRepository.SaveAsync(_mailgun);
    long version = _mailgun.Version;

    DisplayNameUnit displayName = new("Logitar Portal");
    _mailgun.DisplayName = displayName;
    _mailgun.Update();
    await _senderRepository.SaveAsync(_mailgun);

    ReplaceSenderPayload payload = new(Faker.Internet.Email())
    {
      DisplayName = " Logitar ",
      Description = "                ",
      Mailgun = new MailgunSettings(MailgunHelper.GenerateApiKey(), Faker.Internet.DomainName())
    };
    ReplaceSenderCommand command = new(_mailgun.Id.ToGuid(), payload, version);
    SenderModel? sender = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(sender);

    Assert.Equal(payload.EmailAddress, sender.EmailAddress);
    Assert.Equal(displayName.Value, sender.DisplayName);
    Assert.Null(sender.Description);
    Assert.Equal(payload.Mailgun, sender.Mailgun);
  }

  [Fact(DisplayName = "It should replace a SendGrid sender.")]
  public async Task It_should_replace_a_SendGrid_sender()
  {
    _sendGrid.DisplayName = new DisplayNameUnit("Logitar");
    _sendGrid.Update();
    await _senderRepository.SaveAsync(_sendGrid);
    long version = _sendGrid.Version;

    DisplayNameUnit displayName = new("Logitar Portal");
    _sendGrid.DisplayName = displayName;
    _sendGrid.Update();
    await _senderRepository.SaveAsync(_sendGrid);

    ReplaceSenderPayload payload = new(Faker.Internet.Email())
    {
      DisplayName = " Logitar ",
      Description = "                ",
      SendGrid = new SendGridSettings(SendGridHelper.GenerateApiKey())
    };
    ReplaceSenderCommand command = new(_sendGrid.Id.ToGuid(), payload, version);
    SenderModel? sender = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(sender);

    Assert.Equal(payload.EmailAddress, sender.EmailAddress);
    Assert.Equal(displayName.Value, sender.DisplayName);
    Assert.Null(sender.Description);
    Assert.Equal(payload.SendGrid, sender.SendGrid);
  }

  [Fact(DisplayName = "It should replace a Twilio sender.")]
  public async Task It_should_replace_a_Twilio_sender()
  {
    _twilio.Phone = new PhoneUnit("+15149873651", countryCode: null, extension: null, isVerified: false);
    _twilio.Update();
    await _senderRepository.SaveAsync(_twilio);
    long version = _twilio.Version;

    PhoneUnit phone = new("+15148422112", countryCode: null, extension: null, isVerified: false);
    _twilio.Phone = phone;
    _twilio.Update();
    await _senderRepository.SaveAsync(_twilio);

    ReplaceSenderPayload payload = new()
    {
      PhoneNumber = " +15149873651 ",
      Description = "                ",
      Twilio = new TwilioSettings(TwilioHelper.GenerateAccountSid(), TwilioHelper.GenerateAuthenticationToken())
    };
    ReplaceSenderCommand command = new(_twilio.Id.ToGuid(), payload, version);
    SenderModel? sender = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(sender);

    Assert.Equal(phone.Number, sender.PhoneNumber);
    Assert.Null(sender.Description);
    Assert.Equal(payload.Twilio, sender.Twilio);
  }

  [Fact(DisplayName = "It should return null when the sender cannot be found.")]
  public async Task It_should_return_null_when_the_sender_cannot_be_found()
  {
    ReplaceSenderPayload payload = new(Faker.Internet.Email())
    {
      SendGrid = new SendGridSettings(SendGridHelper.GenerateApiKey())
    };
    ReplaceSenderCommand command = new(Guid.NewGuid(), payload, Version: null);
    SenderModel? sender = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(sender);
  }

  [Fact(DisplayName = "It should return null when the sender is in another tenant.")]
  public async Task It_should_return_null_when_the_sender_is_in_another_tenant()
  {
    SetRealm();

    ReplaceSenderPayload payload = new(Faker.Internet.Email())
    {
      SendGrid = new SendGridSettings(SendGridHelper.GenerateApiKey())
    };
    ReplaceSenderCommand command = new(_sendGrid.Id.ToGuid(), payload, Version: null);
    SenderModel? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceSenderPayload payload = new("aa@@bb..cc")
    {
      SendGrid = new SendGridSettings(SendGridHelper.GenerateApiKey())
    };
    ReplaceSenderCommand command = new(_sendGrid.Id.ToGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("EmailAddress", exception.Errors.Single().PropertyName);
  }
}
