using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Client;

internal class TestContext
{
  private int _index = 1;
  private int _success = 0;

  private Realm? _realm = null;

  private readonly int _count;

  public TestContext(int count)
  {
    _count = count;
  }

  public bool HasFailed { get; private set; }
  public Realm Realm
  {
    get => _realm ?? throw new InvalidOperationException("The realm has not been assigned yet.");
    set => _realm = value;
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
  }
}
