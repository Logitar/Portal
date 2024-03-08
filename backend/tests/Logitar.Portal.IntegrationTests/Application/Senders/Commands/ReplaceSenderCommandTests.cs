using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.SendGrid;
using Logitar.Portal.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Senders.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceSenderCommandTests : IntegrationTests
{
  private readonly ISenderRepository _senderRepository;

  private readonly SenderAggregate _sender;

  public ReplaceSenderCommandTests() : base()
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

  [Fact(DisplayName = "It should replace an existing sender.")]
  public async Task It_should_replace_an_existing_sender()
  {
    _sender.DisplayName = new DisplayNameUnit("Reset Password");
    _sender.Update();
    await _senderRepository.SaveAsync(_sender);
    long version = _sender.Version;

    DisplayNameUnit displayName = new("Password Recovery");
    _sender.DisplayName = displayName;
    _sender.Update();
    await _senderRepository.SaveAsync(_sender);

    ReplaceSenderPayload payload = new(Faker.Internet.Email())
    {
      DisplayName = " Default Sender ",
      Description = "                ",
      SendGrid = new SendGridSettings(SendGridHelper.GenerateApiKey())
    };
    ReplaceSenderCommand command = new(_sender.Id.ToGuid(), payload, version);
    Sender? sender = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(sender);

    Assert.Equal(payload.EmailAddress, sender.EmailAddress);
    Assert.Equal(payload.DisplayName.Trim(), sender.DisplayName);
    Assert.Null(sender.Description);
    Assert.Equal(payload.SendGrid, sender.SendGrid);
  }

  [Fact(DisplayName = "It should return null when the sender cannot be found.")]
  public async Task It_should_return_null_when_the_sender_cannot_be_found()
  {
    ReplaceSenderPayload payload = new(Faker.Internet.Email())
    {
      SendGrid = new SendGridSettings(SendGridHelper.GenerateApiKey())
    };
    ReplaceSenderCommand command = new(Guid.NewGuid(), payload, Version: null);
    Sender? sender = await ActivityPipeline.ExecuteAsync(command);
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
    ReplaceSenderCommand command = new(_sender.Id.ToGuid(), payload, Version: null);
    Sender? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceSenderPayload payload = new("aa@@bb..cc")
    {
      SendGrid = new SendGridSettings(SendGridHelper.GenerateApiKey())
    };
    ReplaceSenderCommand command = new(Guid.NewGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("EmailAddress", exception.Errors.Single().PropertyName);
  }
}
