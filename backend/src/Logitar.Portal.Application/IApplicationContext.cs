using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Application;

public interface IApplicationContext
{
  public Actor Actor { get; }
  ActorId ActorId { get; }

  public Configuration Configuration { get; }
  public Realm? Realm { get; }

  public IRoleSettings RoleSettings { get; }
  public IUserSettings UserSettings { get; }
}
