using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal;

internal class TestContext
{
  private readonly int _count;
  private int _index = 1;
  private int _success = 0;
  private int Percentage => _index * 100 / _count;

  private bool _hasEnded = false;

  public bool HasFailed { get; private set; }

  public CancellationToken CancellationToken { get; }

  private string? _name = null;
  public string Name => _name ?? throw new InvalidOperationException($"The {nameof(Name)} has not been initialized yet.");
  public void SetName(Type clientType, string methodName)
  {
    AssertHasNotEnded();
    _name = string.Join('.', clientType.Name, methodName);
  }

  public IRequestContext Request => new RequestContext(_user?.UniqueName, CancellationToken);

  private Realm? _realm = null;
  public Realm Realm => _realm ?? throw new InvalidOperationException($"The {nameof(Realm)} has not been initialized yet.");
  public void SetRealm(Realm realm)
  {
    AssertHasNotEnded();
    _realm = realm;
    StaticPortalSettings.Instance.Realm = realm.UniqueSlug;
  }

  private Role? _role = null;
  public Role Role => _role ?? throw new InvalidOperationException($"The {nameof(Role)} has not been initialized yet.");
  public void SetRole(Role role)
  {
    AssertHasNotEnded();
    _role = role;
  }

  private User? _user = null;
  public User User => _user ?? throw new InvalidOperationException($"The {nameof(User)} has not been initialized yet.");
  public void SetUser(User user)
  {
    AssertHasNotEnded();
    _user = user;
  }

  private string? _token = null;
  public string Token => _token ?? throw new InvalidOperationException($"The {nameof(Token)} has not been initialized yet.");
  public void SetToken(string token)
  {
    AssertHasNotEnded();
    _token = token;
  }

  private Template? _template = null;
  public Template Template => _template ?? throw new InvalidOperationException($"The {nameof(Template)} has not been initialized yet.");
  public void SetTemplate(Template template)
  {
    AssertHasNotEnded();
    _template = template;
  }

  public void Reset()
  {
    AssertHasNotEnded();
    _realm = null;
    _role = null;
    _user = null;
    _token = null;
    _template = null;
    StaticPortalSettings.Instance.Realm = null;
  }

  public static TestContext Start(int count, CancellationToken cancellationToken = default)
  {
    Console.WriteLine("Test run starting.");
    Console.WriteLine();

    return new TestContext(count, cancellationToken);
  }
  private TestContext(int count, CancellationToken cancellationToken)
  {
    _count = count;
    CancellationToken = cancellationToken;
  }

  public void End()
  {
    AssertHasNotEnded();

    Console.WriteLine();
    int success = _success * 100 / _count;
    Console.WriteLine("Test run {0}. | Success rate: {1}%", success == 100 ? "succeeded" : "failed", success);
    Console.WriteLine();

    _hasEnded = true;
  }

  public void Succeed()
  {
    AssertHasNotEnded();

    Console.WriteLine("[{0}%] ({1}/{2}) ✓ Test '{3}' succeeded", Percentage, _index, _count, Name);

    _index++;
    _success++;
    _name = null;
  }
  public void Fail(Exception? exception = null)
  {
    AssertHasNotEnded();

    Console.WriteLine("[{0}%] ({1}/{2}) X Test '{3}' failed", Percentage, _index, _count, Name);
    if (exception != null)
    {
      Console.WriteLine(exception);
    }

    HasFailed = true;
    _name = null;
  }

  private void AssertHasNotEnded()
  {
    if (_hasEnded)
    {
      throw new InvalidOperationException("The test sequence has already ended.");
    }
  }
}
