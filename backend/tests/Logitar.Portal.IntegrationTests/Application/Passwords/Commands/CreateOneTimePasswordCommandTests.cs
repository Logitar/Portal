using Logitar.Data;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Contracts.Passwords;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Application.OneTimePasswords.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateOneTimePasswordCommandTests : IntegrationTests
{
  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    TableId[] tables = [IdentityDb.OneTimePasswords.Table];
    foreach (TableId table in tables)
    {
      ICommand command = CreateDeleteBuilder(table).Build();
      await PortalContext.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray());
    }
  }

  [Fact(DisplayName = "It should create a new One-Time Password.")]
  public async Task It_should_create_a_new_One_Time_Password()
  {
    SetRealm();

    CreateOneTimePasswordPayload payload = new(characters: "ACDEFGHJKLMNPQRSTUVWXYZ2345679", length: 6)
    {
      ExpiresOn = DateTime.Now.AddMinutes(1),
      MaximumAttempts = 5
    };
    payload.CustomAttributes.Add(new("UserId", Guid.NewGuid().ToString()));
    CreateOneTimePasswordCommand command = new(payload);
    OneTimePassword oneTimePassword = await ActivityPipeline.ExecuteAsync(command);

    Assertions.Equal(payload.ExpiresOn, oneTimePassword.ExpiresOn, TimeSpan.FromSeconds(1));
    Assert.Equal(payload.MaximumAttempts, oneTimePassword.MaximumAttempts);
    Assert.Equal(0, oneTimePassword.AttemptCount);
    Assert.False(oneTimePassword.HasValidationSucceeded);
    Assert.Equal(payload.CustomAttributes, oneTimePassword.CustomAttributes);
    Assert.Same(Realm, oneTimePassword.Realm);

    Assert.NotNull(oneTimePassword.Password);
    Assert.All(oneTimePassword.Password, c => Assert.Contains(c, payload.Characters));
    Assert.Equal(payload.Length, oneTimePassword.Password.Length);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateOneTimePasswordPayload payload = new(characters: string.Empty, length: byte.MaxValue);
    CreateOneTimePasswordCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("Characters", exception.Errors.Single().PropertyName);
  }
}
