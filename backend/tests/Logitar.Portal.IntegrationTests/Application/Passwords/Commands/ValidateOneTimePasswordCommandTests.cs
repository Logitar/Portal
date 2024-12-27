using Logitar.Data;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Portal.Contracts.Passwords;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Passwords.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ValidateOneTimePasswordCommandTests : IntegrationTests
{
  private readonly IOneTimePasswordRepository _oneTimePasswordRepository;
  private readonly IPasswordManager _passwordManager;

  private string? _password = null;

  public ValidateOneTimePasswordCommandTests() : base()
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

  [Fact(DisplayName = "It should return null when the One-Time Password cannot be found.")]
  public async Task It_should_return_null_when_the_One_Time_Password_cannot_be_found()
  {
    ValidateOneTimePasswordPayload payload = new("P@s$W0rD");
    ValidateOneTimePasswordCommand command = new(Guid.NewGuid(), payload);
    OneTimePasswordModel? oneTimePassword = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(oneTimePassword);
  }

  [Fact(DisplayName = "It should return null when the One-Time Password is in another tenant.")]
  public async Task It_should_return_null_when_the_One_Time_Password_is_in_another_tenant()
  {
    OneTimePassword oneTimePassword = await CreateOneTimePasswordAsync();

    SetRealm();

    ValidateOneTimePasswordPayload payload = new("P@s$W0rD");
    ValidateOneTimePasswordCommand command = new(oneTimePassword.Id.ToGuid(), payload);
    OneTimePasswordModel? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(result);
  }

  [Fact(DisplayName = "It should throw IncorrectOneTimePasswordPasswordException when the password is not correct.")]
  public async Task It_should_throw_IncorrectOneTimePasswordPasswordException_when_the_password_is_not_correct()
  {
    OneTimePassword oneTimePassword = await CreateOneTimePasswordAsync();

    ValidateOneTimePasswordPayload payload = new("P@s$W0rD");
    ValidateOneTimePasswordCommand command = new(oneTimePassword.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<IncorrectOneTimePasswordPasswordException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(oneTimePassword.Id, exception.OneTimePasswordId);
    Assert.Equal(payload.Password, exception.AttemptedPassword);
  }

  [Fact(DisplayName = "It should throw MaximumAttemptsReachedException when the maximum number of attempts has been reached.")]
  public async Task It_should_throw_MaximumAttemptsReachedException_when_the_maximum_number_of_attempts_has_been_reached()
  {
    OneTimePassword oneTimePassword = await CreateOneTimePasswordAsync(maximumAttempts: 1);

    ValidateOneTimePasswordPayload payload = new("P@s$W0rD");
    ValidateOneTimePasswordCommand command = new(oneTimePassword.Id.ToGuid(), payload);
    _ = await Assert.ThrowsAsync<IncorrectOneTimePasswordPasswordException>(async () => await ActivityPipeline.ExecuteAsync(command));
    var exception = await Assert.ThrowsAsync<MaximumAttemptsReachedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(oneTimePassword.Id, exception.OneTimePasswordId);
    Assert.Equal(1, exception.AttemptCount);
  }

  [Fact(DisplayName = "It should throw OneTimePasswordAlreadyUsedException when validation has already succeeded.")]
  public async Task It_should_throw_OneTimePasswordAlreadyUsedException_when_validation_has_already_succeeded()
  {
    OneTimePassword oneTimePassword = await CreateOneTimePasswordAsync();
    Assert.NotNull(_password);

    oneTimePassword.Validate(_password);
    await _oneTimePasswordRepository.SaveAsync(oneTimePassword);

    ValidateOneTimePasswordPayload payload = new("P@s$W0rD");
    ValidateOneTimePasswordCommand command = new(oneTimePassword.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<OneTimePasswordAlreadyUsedException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(oneTimePassword.Id, exception.OneTimePasswordId);
  }

  [Fact(DisplayName = "It should throw OneTimePasswordIsExpiredException when the One-Time Password is expired.")]
  public async Task It_should_throw_OneTimePasswordIsExpiredException_when_the_One_Time_Password_is_expired()
  {
    const int millisecondsDelay = 500;

    OneTimePassword oneTimePassword = await CreateOneTimePasswordAsync(expiresOn: DateTime.Now.AddMilliseconds(millisecondsDelay));
    Assert.NotNull(_password);

    await Task.Delay(millisecondsDelay);

    ValidateOneTimePasswordPayload payload = new(_password);
    ValidateOneTimePasswordCommand command = new(oneTimePassword.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<OneTimePasswordIsExpiredException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal(oneTimePassword.Id, exception.OneTimePasswordId);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ValidateOneTimePasswordPayload payload = new(string.Empty);
    ValidateOneTimePasswordCommand command = new(Guid.NewGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await ActivityPipeline.ExecuteAsync(command));
    Assert.Equal("Password", exception.Errors.Single().PropertyName);
  }

  [Fact(DisplayName = "It should validate a One-Time Password.")]
  public async Task It_should_validate_a_One_Time_Password()
  {
    OneTimePassword oneTimePassword = await CreateOneTimePasswordAsync();
    Assert.NotNull(_password);

    ValidateOneTimePasswordPayload payload = new(_password);
    payload.CustomAttributes.Add(new("ValidatedBy", UsernameString));
    payload.CustomAttributes.Add(new("ValidatedOn", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()));
    ValidateOneTimePasswordCommand command = new(oneTimePassword.Id.ToGuid(), payload);
    OneTimePasswordModel? result = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(result);
    Assert.Equal(1, result.AttemptCount);
    Assert.True(result.HasValidationSucceeded);
    Assert.Equal(payload.CustomAttributes, result.CustomAttributes);
  }

  private async Task<OneTimePassword> CreateOneTimePasswordAsync(DateTime? expiresOn = null, int? maximumAttempts = null)
  {
    Password password = _passwordManager.Generate("0123456789", 6, out _password);
    OneTimePassword oneTimePassword = new(password, tenantId: null, expiresOn, maximumAttempts);

    await _oneTimePasswordRepository.SaveAsync(oneTimePassword);

    return oneTimePassword;
  }
}
