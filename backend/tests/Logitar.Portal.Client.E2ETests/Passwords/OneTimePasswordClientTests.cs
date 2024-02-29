using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Passwords;

namespace Logitar.Portal.OneTimePasswords;

internal class OneTimePasswordClientTests : IClientTests
{
  private readonly IOneTimePasswordClient _client;

  public OneTimePasswordClientTests(IOneTimePasswordClient client)
  {
    _client = client;
  }

  public async Task<bool> ExecuteAsync(TestContext context)
  {
    try
    {
      context.SetName(_client.GetType(), nameof(_client.CreateAsync));
      if (context.User == null)
      {
        throw new InvalidOperationException("The user should not be null in the context.");
      }
      CreateOneTimePasswordPayload create = new("1234567890", 6)
      {
        ExpiresOn = DateTime.Now.AddHours(1),
        MaximumAttempts = 5
      };
      create.CustomAttributes.Add(new CustomAttribute("Purpose", "MultiFactorAuthentication"));
      create.CustomAttributes.Add(new CustomAttribute("UserId", context.User.Id.ToString()));
      OneTimePassword oneTimePassword = await _client.CreateAsync(create, context.Request);
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.DeleteAsync));
      oneTimePassword = await _client.DeleteAsync(oneTimePassword.Id, context.Request)
        ?? throw new InvalidOperationException("The One-Time Password (OTP) should not be null.");
      oneTimePassword = await _client.CreateAsync(create, context.Request);
      string password = oneTimePassword.Password ?? throw new InvalidOperationException("The One-Time Password (OTP) should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.ReadAsync));
      OneTimePassword? notFound = await _client.ReadAsync(Guid.NewGuid(), context.Request);
      if (notFound != null)
      {
        throw new InvalidOperationException("The One-Time Password (OTP) should not be found.");
      }
      oneTimePassword = await _client.ReadAsync(oneTimePassword.Id, context.Request)
        ?? throw new InvalidOperationException("The One-Time Password (OTP) should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.ValidateAsync));
      ValidateOneTimePasswordPayload validate = new(password);
      oneTimePassword = await _client.ValidateAsync(oneTimePassword.Id, validate, context.Request)
        ?? throw new InvalidOperationException("The One-Time Password (OTP) should not be null.");
      context.Succeed();
    }
    catch (Exception exception)
    {
      context.Fail(exception);
    }

    return !context.HasFailed;
  }
}
