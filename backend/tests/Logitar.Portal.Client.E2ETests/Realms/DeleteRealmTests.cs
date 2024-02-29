using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Realms;

internal class DeleteRealmTests : IClientTests
{
  private readonly IRealmClient _client;

  public DeleteRealmTests(IRealmClient client)
  {
    _client = client;
  }

  public async Task<bool> ExecuteAsync(TestContext context)
  {
    try
    {
      context.SetName(GetType(), nameof(ExecuteAsync));
      Realm realm = await _client.DeleteAsync(context.Realm.Id, context.Request)
        ?? throw new InvalidOperationException("The realm should not be null.");
      context.Reset();
      context.Succeed();
    }
    catch (Exception exception)
    {
      context.Fail(exception);
    }

    return !context.HasFailed;
  }
}
