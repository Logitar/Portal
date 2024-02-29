namespace Logitar.Portal;

internal interface IClientTests
{
  Task<bool> ExecuteAsync(TestContext context);
}
