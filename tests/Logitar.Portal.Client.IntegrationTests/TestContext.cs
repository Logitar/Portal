using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Client;

internal class TestContext
{
  private int _index = 1;
  private int _success = 0;

  private readonly int _count;

  public TestContext(int count)
  {
    _count = count;
  }

  public bool HasFailed { get; private set; }

  private Realm? _realm = null;
  public Realm Realm
  {
    get => _realm ?? throw new InvalidOperationException("The realm has not been assigned yet.");
    set => _realm = value;
  }

  private Role? _role = null;
  public Role Role
  {
    get => _role ?? throw new InvalidOperationException("The role has not been assigned yet.");
    set => _role = value;
  }

  private User? _user = null;
  public User User
  {
    get => _user ?? throw new InvalidOperationException("The user has not been assigned yet.");
    set => _user = value;
  }

  private Template? _template = null;
  public Template Template
  {
    get => _template ?? throw new InvalidOperationException("The template has not been assigned yet.");
    set => _template = value;
  }

  private int Percentage => _index * 100 / _count;

  public virtual void Start()
  {
    Console.WriteLine("Test run starting.");
    Console.WriteLine();
  }

  public void Succeed(string name)
  {
    Console.WriteLine("[{0}%] ({1}/{2}) ✓ Test '{3}' succeeded", Percentage, _index, _count, name);

    _index++;
    _success++;
  }

  public void Fail(string name, Exception exception)
  {
    Console.WriteLine("[{0}%] ({1}/{2}) X Test '{3}' failed", Percentage, _index, _count, name);
    Console.WriteLine(exception);

    _index++;

    HasFailed = true;
  }

  public void End()
  {
    int success = _success * 100 / _count;

    Console.WriteLine();
    Console.WriteLine("Test run {0}. | Success rate: {1}%", success == 100 ? "succeeded" : "failed", success);
    Console.WriteLine();
    Console.Write("Press a key to close the console.");
    Console.ReadKey(intercept: true);
  }
}
