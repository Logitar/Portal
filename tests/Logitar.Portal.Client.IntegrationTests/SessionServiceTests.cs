using Bogus;
using Logitar.Portal.Client.Implementations;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Client;

internal class SessionServiceTests
{
  private readonly Faker _faker = new();

  private readonly TestContext _context;
  private readonly ISessionService _sessionService;

  public SessionServiceTests(TestContext context, ISessionService sessionService)
  {
    _context = context;
    _sessionService = sessionService;
  }

  public async Task<Session?> ExecuteAsync(Credentials credentials, CancellationToken cancellationToken = default)
  {
    string name = string.Empty;
    try
    {
      string realm = _context.Realm.Id.ToString();

      name = string.Join('.', nameof(SessionService), nameof(SessionService.SignInAsync));
      Session session = await _sessionService.SignInAsync(new SignInInput
      {
        Username = credentials.Username,
        Password = credentials.Password
      }, realm, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(SessionService), nameof(SessionService.SignOutAsync));
      session = await _sessionService.SignOutAsync(session.Id, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(SessionService), nameof(SessionService.RefreshAsync));
      session = await _sessionService.SignInAsync(new SignInInput
      {
        Username = credentials.Username,
        Password = credentials.Password,
        Remember = true
      }, realm, cancellationToken);
      session = await _sessionService.RefreshAsync(new RefreshInput
      {
        RefreshToken = session.RefreshToken ?? string.Empty
      }, cancellationToken);
      _context.Succeed(name);

      name = string.Join('.', nameof(SessionService), nameof(SessionService.GetAsync));
      session = (await _sessionService.GetAsync(isActive: true, cancellationToken: cancellationToken)).Items.Single();
      _context.Succeed(name);

      name = string.Join('.', nameof(SessionService), $"{nameof(SessionService.GetAsync)}(id)");
      session = await _sessionService.GetAsync(session.Id, cancellationToken: cancellationToken)
        ?? throw new InvalidOperationException("The result cannot be null.");
      _context.Succeed(name);

      name = string.Join('.', nameof(SessionService), nameof(SessionService.SignOutUserAsync));
      session = (await _sessionService.SignOutUserAsync(session.User.Id, cancellationToken)).Single();
      _context.Succeed(name);

      return session;
    }
    catch (Exception exception)
    {
      _context.Fail(name, exception);

      return null;
    }
  }
}
