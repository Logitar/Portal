using Logitar.Data;
using Logitar.Data.SqlServer;
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

  [Fact(DisplayName = "It should create a new sender.")]
  public async Task It_should_create_a_new_sender()
  {
    CreateSenderPayload payload = new(Faker.Internet.Email())
    {
      DisplayName = "Default Sender",
      Description = "    ",
      SendGrid = new SendGridSettings(SendGridHelper.GenerateApiKey())
    };
    CreateSenderCommand command = new(payload);
    Sender sender = await Mediator.Send(command);

    Assert.True(sender.IsDefault);
    Assert.Equal(payload.EmailAddress, sender.EmailAddress);
    Assert.Equal(payload.DisplayName, sender.DisplayName);
    Assert.Null(sender.Description);
    Assert.Equal(SenderProvider.SendGrid, sender.Provider);
    Assert.Equal(payload.SendGrid, sender.SendGrid);
    Assert.Null(sender.Realm);

    SetRealm();
    Sender other1 = await Mediator.Send(command);
    Assert.Same(Realm, other1.Realm);
    Assert.True(other1.IsDefault);

    Sender other2 = await Mediator.Send(command);
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
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Mediator.Send(command));
    Assert.Equal("EmailAddress", exception.Errors.Single().PropertyName);
  }
}
