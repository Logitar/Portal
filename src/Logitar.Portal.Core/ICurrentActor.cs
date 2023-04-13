using Logitar.EventSourcing;

namespace Logitar.Portal.Core;

public interface ICurrentActor
{
  AggregateId Id { get; }
}
