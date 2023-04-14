using Logitar.EventSourcing;

namespace Logitar.Portal.Core.Realms;

public class CannotManagePortalRealmException : Exception
{
  public CannotManagePortalRealmException(AggregateId actorId)
    : base($"The Portal realm cannot be managed (ActorId={actorId}).")
  {
    Data["ActorId"] = actorId.Value;
  }
}
