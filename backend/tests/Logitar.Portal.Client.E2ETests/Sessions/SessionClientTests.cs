using Logitar.Portal.Client;
using Logitar.Portal.Contracts.Sessions;
using Microsoft.Extensions.Configuration;

namespace Logitar.Portal.Sessions;

internal class SessionClientTests : IClientTests
{
  private readonly ISessionClient _client;
  private readonly BasicCredentials _credentials;

  public SessionClientTests(ISessionClient client, IConfiguration configuration)
  {
    _client = client;

    PortalSettings portalSettings = configuration.GetSection("Portal").Get<PortalSettings>() ?? new();
    _credentials = portalSettings.Basic ?? throw new ArgumentException("The configuration 'Portal.Basic' is required.", nameof(configuration));
  }

  public async Task<bool> ExecuteAsync(TestContext context)
  {
    try
    {
      context.SetName(_client.GetType(), nameof(_client.SignInAsync));
      SignInSessionPayload signIn = new(_credentials.Username, _credentials.Password);
      Session session = await _client.SignInAsync(signIn, context.Request);
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.SignOutAsync));
      session = await _client.SignOutAsync(session.Id, context.Request)
        ?? throw new InvalidOperationException("The session should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.CreateAsync));
      CreateSessionPayload create = new(_credentials.Username, isPersistent: true, []);
      session = await _client.CreateAsync(create, context.Request);
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.RenewAsync));
      if (session.RefreshToken == null)
      {
        throw new InvalidOperationException("The session refresh token should not be null.");
      }
      RenewSessionPayload renew = new(session.RefreshToken);
      session = await _client.RenewAsync(renew, context.Request);
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.ReadAsync));
      Session? notFound = await _client.ReadAsync(Guid.NewGuid(), context.Request);
      if (notFound != null)
      {
        throw new InvalidOperationException("The session should not be found.");
      }
      session = await _client.ReadAsync(session.Id, context.Request)
        ?? throw new InvalidOperationException("The session should not be null.");
      context.Succeed();

      context.SetName(_client.GetType(), nameof(_client.SearchAsync));
      SearchSessionsPayload search = new()
      {
        UserId = context.User.Id,
        IsActive = true,
        IsPersistent = true
      };
      context.Succeed();
    }
    catch (Exception exception)
    {
      context.Fail(exception);
    }

    return !context.HasFailed;
  }
}
