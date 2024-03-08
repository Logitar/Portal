using Logitar.Portal.Application;

namespace Logitar.Portal;

internal class TestContextParametersResolver : IContextParametersResolver
{
  private readonly TestContext _context;

  public TestContextParametersResolver(TestContext context)
  {
    _context = context;
  }

  public IContextParameters Resolve() => new ContextParameters
  {
    Realm = _context.Realm,
    ApiKey = _context.ApiKey,
    User = _context.User,
    Session = _context.Session
  };
}
