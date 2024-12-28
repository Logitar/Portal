using Logitar.Data;
using Logitar.Identity.Core.Passwords;
using Logitar.Portal.Contracts.Passwords;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IdentityDb = Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;

namespace Logitar.Portal.Application.Passwords.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateOneTimePasswordCommandTests : IntegrationTests
{
  private readonly IOneTimePasswordRepository _oneTimePasswordRepository;
  private readonly IPasswordManager _passwordManager;

  public CreateOneTimePasswordCommandTests() : base()
  {
    _oneTimePasswordRepository = ServiceProvider.GetRequiredService<IOneTimePasswordRepository>();
    _passwordManager = ServiceProvider.GetRequiredService<IPasswordManager>();
  }

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

  [Theory(DisplayName = "It should create a new One-Time Password.")]
  [InlineData(null)]
  [InlineData("cb47561c-a65f-4e8d-9031-99b1a2e02fb3")]
  public async Task It_should_create_a_new_One_Time_Password(string? idValue)
  {
    SetRealm();

    CreateOneTimePasswordPayload payload = new(characters: "ACDEFGHJKLMNPQRSTUVWXYZ2345679", length: 6)
    {
      ExpiresOn = DateTime.Now.AddMinutes(1),
      MaximumAttempts = 5
    };
    if (idValue != null)
    {
      payload.Id = Guid.Parse(idValue);
    }
    payload.CustomAttributes.Add(new("UserId", Guid.NewGuid().ToString()));
    CreateOneTimePasswordCommand command = new(payload);
    OneTimePasswordModel oneTimePassword = await ActivityPipeline.ExecuteAsync(command);

    if (payload.Id.HasValue)
    {
      Assert.Equal(payload.Id.Value, oneTimePassword.Id);
    }
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

  [Fact(DisplayName = "It should throw IdAlreadyUsedException when the ID is already taken.")]
  public async Task It_should_throw_IdAlreadyUsedException_when_the_Id_is_already_taken()
  {
    Password password = _passwordManager.Generate("1234567890", 6, out _);
    OneTimePassword oneTimePassword = new(password);
    await _oneTimePasswordRepository.SaveAsync(oneTimePassword);

    CreateOneTimePasswordPayload payload = new("1234567890", 6)
    {
      Id = oneTimePassword.EntityId.ToGuid()
    };
    CreateOneTimePasswordCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<IdAlreadyUsedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(payload.Id.Value, exception.Id);
    Assert.Equal("Id", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateOneTimePasswordPayload payload = new(characters: string.Empty, length: byte.MaxValue)
    {
      Id = Guid.Empty
    };
    CreateOneTimePasswordCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Id.Value");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Characters");
  }
}
