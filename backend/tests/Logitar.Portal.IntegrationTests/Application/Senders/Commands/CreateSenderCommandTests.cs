using Logitar.Data;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.SendGrid;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortalDb = Logitar.Portal.EntityFrameworkCore.Relational.PortalDb;

namespace Logitar.Portal.Application.Senders.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateSenderCommandTests : IntegrationTests
{
  private readonly ISenderRepository _senderRepository;

  public CreateSenderCommandTests() : base()
  {
    _senderRepository = ServiceProvider.GetRequiredService<ISenderRepository>();
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
  }

  [Theory(DisplayName = "It should create a new Mailgun sender.")]
  [InlineData(null)]
  [InlineData("eee282b2-bb35-4ce8-b001-f463d77360e8")]
  public async Task It_should_create_a_new_Mailgun_sender(string? idValue)
  {
    CreateSenderPayload payload = new(Faker.Internet.Email())
    {
      DisplayName = "Alternative Sender",
      Description = "    ",
      Mailgun = new MailgunSettings(MailgunHelper.GenerateApiKey(), Faker.Internet.DomainName())
    };
    if (idValue != null)
    {
      payload.Id = Guid.Parse(idValue);
    }
    CreateSenderCommand command = new(payload);
    SenderModel sender = await ActivityPipeline.ExecuteAsync(command);

    if (payload.Id.HasValue)
    {
      Assert.Equal(payload.Id.Value, sender.Id);
    }
    Assert.True(sender.IsDefault);
    Assert.Equal(payload.EmailAddress, sender.EmailAddress);
    Assert.Null(sender.PhoneNumber);
    Assert.Equal(payload.DisplayName, sender.DisplayName);
    Assert.Null(sender.Description);
    Assert.Equal(SenderProvider.Mailgun, sender.Provider);
    Assert.Equal(payload.Mailgun, sender.Mailgun);
    Assert.Null(sender.Realm);

    SetRealm();
    SenderModel other1 = await ActivityPipeline.ExecuteAsync(command);
    Assert.Same(Realm, other1.Realm);
    Assert.True(other1.IsDefault);

    payload.Id = null;
    SenderModel other2 = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotEqual(other1, other2);
    Assert.Same(Realm, other1.Realm);
    Assert.False(other2.IsDefault);
  }

  [Theory(DisplayName = "It should create a new SendGrid sender.")]
  [InlineData(null)]
  [InlineData("aa3dc1fa-dd03-45cb-bc42-9c56bc0ede43")]
  public async Task It_should_create_a_new_SendGrid_sender(string? idValue)
  {
    CreateSenderPayload payload = new(Faker.Internet.Email())
    {
      DisplayName = "Default Sender",
      Description = "    ",
      SendGrid = new SendGridSettings(SendGridHelper.GenerateApiKey())
    };
    if (idValue != null)
    {
      payload.Id = Guid.Parse(idValue);
    }
    CreateSenderCommand command = new(payload);
    SenderModel sender = await ActivityPipeline.ExecuteAsync(command);

    if (payload.Id.HasValue)
    {
      Assert.Equal(payload.Id.Value, sender.Id);
    }
    Assert.True(sender.IsDefault);
    Assert.Equal(payload.EmailAddress, sender.EmailAddress);
    Assert.Null(sender.PhoneNumber);
    Assert.Equal(payload.DisplayName, sender.DisplayName);
    Assert.Null(sender.Description);
    Assert.Equal(SenderProvider.SendGrid, sender.Provider);
    Assert.Equal(payload.SendGrid, sender.SendGrid);
    Assert.Null(sender.Realm);

    SetRealm();
    SenderModel other1 = await ActivityPipeline.ExecuteAsync(command);
    Assert.Same(Realm, other1.Realm);
    Assert.True(other1.IsDefault);

    payload.Id = null;
    SenderModel other2 = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotEqual(other1, other2);
    Assert.Same(Realm, other1.Realm);
    Assert.False(other2.IsDefault);
  }

  [Theory(DisplayName = "It should create a new Twilio sender.")]
  [InlineData(null)]
  [InlineData("0cd62890-27b6-4c93-9219-9b4e90452fce")]
  public async Task It_should_create_a_new_Twilio_sender(string? idValue)
  {
    CreateSenderPayload payload = new()
    {
      PhoneNumber = "+15148454636",
      Description = "    ",
      Twilio = new TwilioSettings(TwilioHelper.GenerateAccountSid(), TwilioHelper.GenerateAuthenticationToken())
    };
    if (idValue != null)
    {
      payload.Id = Guid.Parse(idValue);
    }
    CreateSenderCommand command = new(payload);
    SenderModel sender = await ActivityPipeline.ExecuteAsync(command);

    if (payload.Id.HasValue)
    {
      Assert.Equal(payload.Id.Value, sender.Id);
    }
    Assert.True(sender.IsDefault);
    Assert.Null(sender.EmailAddress);
    Assert.Equal(payload.PhoneNumber, sender.PhoneNumber);
    Assert.Null(sender.DisplayName);
    Assert.Null(sender.Description);
    Assert.Equal(SenderProvider.Twilio, sender.Provider);
    Assert.Equal(payload.Twilio, sender.Twilio);
    Assert.Null(sender.Realm);

    SetRealm();
    SenderModel other1 = await ActivityPipeline.ExecuteAsync(command);
    Assert.Same(Realm, other1.Realm);
    Assert.True(other1.IsDefault);

    payload.Id = null;
    SenderModel other2 = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotEqual(other1, other2);
    Assert.Same(Realm, other1.Realm);
    Assert.False(other2.IsDefault);
  }

  [Fact(DisplayName = "It should throw IdAlreadyUsedException when the ID is already taken.")]
  public async Task It_should_throw_IdAlreadyUsedException_when_the_Id_is_already_taken()
  {
    Email email = new(Faker.Internet.Email());
    ReadOnlySendGridSettings settings = new(SendGridHelper.GenerateApiKey());
    Sender sender = new(email, settings);
    await _senderRepository.SaveAsync(sender);

    CreateSenderPayload payload = new(email.Address)
    {
      Id = sender.EntityId.ToGuid(),
      SendGrid = new SendGridSettings(settings.ApiKey)
    };
    CreateSenderCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<IdAlreadyUsedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(payload.Id.Value, exception.Id);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateSenderPayload payload = new("aa@@bb..cc")
    {
      Id = Guid.Empty,
      SendGrid = new SendGridSettings(SendGridHelper.GenerateApiKey())
    };
    CreateSenderCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Id.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EmailValidator" && e.PropertyName == "EmailAddress");
  }
}
