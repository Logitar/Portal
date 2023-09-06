using Bogus;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Client;

internal class SessionClientTests
{
  private const string PasswordString = "P@s$W0rD";
  private const string Sut = "SessionClient";

  private readonly TestContext _context;
  private readonly Faker _faker;
  private readonly ISessionService _sessionService;

  public SessionClientTests(TestContext context, Faker faker, ISessionService sessionService)
  {
    _context = context;
    _faker = faker;
    _sessionService = sessionService;
  }

  public async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
  {
    string name = string.Empty;

    try
    {
      name = $"{Sut}.{nameof(_sessionService.CreateAsync)}";
      CreateSessionPayload create = new()
      {
        UserId = _context.User.Id,
        CustomAttributes = new CustomAttribute[]
        {
          new("AdditionalInformation", $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}"),
          new("IpAddress", _faker.Internet.Ip())
        }
      };
      Session session = await _sessionService.CreateAsync(create, cancellationToken);
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_sessionService.SignInAsync)}";
      SignInPayload signIn = new()
      {
        Realm = $"  {_context.Realm.UniqueSlug.ToUpper()}  ",
        UniqueName = $"  {_context.User.UniqueName.ToUpper()}  ",
        Password = PasswordString,
        IsPersistent = true,
        CustomAttributes = new CustomAttribute[]
        {
          new("AdditionalInformation", $@"{{""User-Agent"":""{_faker.Internet.UserAgent()}""}}"),
          new("IpAddress", _faker.Internet.Ip())
        }
      };
      session = await _sessionService.SignInAsync(signIn, cancellationToken);
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_sessionService.RenewAsync)}";
      RenewPayload renew = new()
      {
        RefreshToken = session.RefreshToken ?? string.Empty,
        CustomAttributes = new CustomAttribute[]
        {
          new("IpAddress", _faker.Internet.Ip())
        }
      };
      session = await _sessionService.RenewAsync(renew, cancellationToken);
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_sessionService.SearchAsync)}";
      SearchSessionsPayload search = new()
      {
        Realm = $"  {_context.Realm.UniqueSlug}  ",
        IdIn = new Guid[] { session.Id },
        IsActive = true,
        IsPersistent = true
      };
      SearchResults<Session> results = await _sessionService.SearchAsync(search, cancellationToken);
      session = results.Results.Single();
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_sessionService.ReadAsync)}:null";
      Session? result = await _sessionService.ReadAsync(Guid.Empty, cancellationToken: cancellationToken);
      if (result != null)
      {
        throw new InvalidOperationException("The session should be null.");
      }
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_sessionService.ReadAsync)}:Id";
      session = await _sessionService.ReadAsync(session.Id, cancellationToken: cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);

      name = $"{Sut}.{nameof(_sessionService.SignOutAsync)}:Id";
      session = await _sessionService.SignOutAsync(session.Id, cancellationToken: cancellationToken)
        ?? throw new InvalidOperationException("The result should not be null.");
      _context.Succeed(name);
    }
    catch (Exception exception)
    {
      _context.Fail(name, exception);
    }

    return !_context.HasFailed;
  }
}
