using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;

namespace Logitar.Portal.Application;

public interface IApplicationContext
{
  ActorId ActorId { get; }

  IRoleSettings RoleSettings { get; }
  IUserSettings UserSettings { get; }
}
