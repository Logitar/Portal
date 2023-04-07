using Logitar.EventSourcing;

namespace Logitar.Portal.v2.Core;

public interface ICurrentActor
{
  AggregateId Id { get; }
}
