using Logitar.Data;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Application.Senders.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateSenderCommandTests : IntegrationTests
{
  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [PortalDb.Senders.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should create a new Mailgun sender.")]
  public async Task It_should_create_a_new_Mailgun_sender()
  {
    CreateSenderPayload payload = new(Faker.Internet.Email())
    {
      DisplayName = "Alternative Sender",
      Description = "    ",
      Mailgun = new MailgunSettings(MailgunHelper.GenerateApiKey(), Faker.Internet.DomainName())
    };
    CreateSenderCommand command = new(payload);
    Sender sender = await ActivityPipeline.ExecuteAsync(command);

    Assert.True(sender.IsDefault);
    Assert.Equal(payload.EmailAddress, sender.EmailAddress);
    Assert.Null(sender.PhoneNumber);
    Assert.Equal(payload.DisplayName, sender.DisplayName);
    Assert.Null(sender.Description);
    Assert.Equal(SenderProvider.Mailgun, sender.Provider);
    Assert.Equal(payload.Mailgun, sender.Mailgun);
    Assert.Null(sender.Realm);

    SetRealm();
    Sender other1 = await ActivityPipeline.ExecuteAsync(command);
    Assert.Same(Realm, other1.Realm);
    Assert.True(other1.IsDefault);

    Sender other2 = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotEqual(other1, other2);
    Assert.Same(Realm, other1.Realm);
    Assert.False(other2.IsDefault);
  }

  [Fact(DisplayName = "It should create a new SendGrid sender.")]
  public async Task It_should_create_a_new_SendGrid_sender()
  {
    CreateSenderPayload payload = new(Faker.Internet.Email())
    {
      DisplayName = "Default Sender",
      Description = "    ",
      SendGrid = new SendGridSettings(SendGridHelper.GenerateApiKey())
    };
    CreateSenderCommand command = new(payload);
    Sender sender = await ActivityPipeline.ExecuteAsync(command);

    Assert.True(sender.IsDefault);
    Assert.Equal(payload.EmailAddress, sender.EmailAddress);
    Assert.Null(sender.PhoneNumber);
    Assert.Equal(payload.DisplayName, sender.DisplayName);
    Assert.Null(sender.Description);
    Assert.Equal(SenderProvider.SendGrid, sender.Provider);
    Assert.Equal(payload.SendGrid, sender.SendGrid);
    Assert.Null(sender.Realm);

    SetRealm();
    Sender other1 = await ActivityPipeline.ExecuteAsync(command);
    Assert.Same(Realm, other1.Realm);
    Assert.True(other1.IsDefault);

    Sender other2 = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotEqual(other1, other2);
    Assert.Same(Realm, other1.Realm);
    Assert.False(other2.IsDefault);
  }

  [Fact(DisplayName = "It should create a new Twilio sender.")]
  public async Task It_should_create_a_new_Twilio_sender()
  {
    CreateSenderPayload payload = new()
    {
      PhoneNumber = "+15148454636",
      Description = "    ",
      Twilio = new TwilioSettings(TwilioHelper.GenerateAccountSid(), TwilioHelper.GenerateAuthenticationToken())
    };
    CreateSenderCommand command = new(payload);
    Sender sender = await ActivityPipeline.ExecuteAsync(command);

    Assert.True(sender.IsDefault);
    Assert.Null(sender.EmailAddress);
    Assert.Equal(payload.PhoneNumber, sender.PhoneNumber);
    Assert.Null(sender.DisplayName);
    Assert.Null(sender.Description);
    Assert.Equal(SenderProvider.Twilio, sender.Provider);
    Assert.Equal(payload.Twilio, sender.Twilio);
    Assert.Null(sender.Realm);

    SetRealm();
    Sender other1 = await ActivityPipeline.ExecuteAsync(command);
    Assert.Same(Realm, other1.Realm);
    Assert.True(other1.IsDefault);

    Sender other2 = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotEqual(other1, other2);
    Assert.Same(Realm, other1.Realm);
    Assert.False(other2.IsDefault);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateSenderPayload payload = new("aa@@bb..cc")
    {
      SendGrid = new SendGridSettings(SendGridHelper.GenerateApiKey())
    };
    CreateSenderCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("EmailAddress", exception.Errors.Single().PropertyName);
  }
}
