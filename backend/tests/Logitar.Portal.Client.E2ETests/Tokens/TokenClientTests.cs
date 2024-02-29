
using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Tokens;

internal class TokenClientTests : IClientTests
{
  private readonly ITokenClient _client;

  public TokenClientTests(ITokenClient client)
  {
    _client = client;
  }

  public async Task<bool> ExecuteAsync(TestContext context)
  {
    try
    {
      context.SetName(_client.GetType(), nameof(_client.CreateAsync));
      CreateTokenPayload create = new()
      {
        IsConsumable = true,
        Type = "reset_password+jwt",
        Subject = context.User.Id.ToString()
      };
      CreatedToken createdToken = await _client.CreateAsync(create, context.Request);
      context.SetToken(createdToken.Token);
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.ValidateAsync));
      ValidateTokenPayload validate = new(createdToken.Token)
      {
        Consume = true,
        Type = create.Type
      };
      _ = await _client.ValidateAsync(validate, context.Request);
      context.Succeed();
    }
    catch (Exception exception)
    {
      context.Fail(exception);
    }

    return !context.HasFailed;
  }
}
