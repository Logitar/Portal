using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Application;

public interface IApplicationContext
{
  ActorId ActorId { get; }

  Realm? Realm { get; }

  IRoleSettings RoleSettings { get; }
  IUserSettings UserSettings { get; }
}
