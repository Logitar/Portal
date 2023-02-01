using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Domain.Realms
{
  public class Realm : AggregateRoot
  {
    private Realm() : base()
    {
    }

    public bool RequireConfirmedAccount { get; private set; }

    public UsernameSettings UsernameSettings { get; private set; } = new();
  }
}
