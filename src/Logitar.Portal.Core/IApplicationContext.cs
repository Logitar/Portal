using Logitar.EventSourcing;
using Logitar.Portal.Core.Configurations;

namespace Logitar.Portal.Core;

public interface IApplicationContext
{
  Guid? ActivityId { get; set; }
  AggregateId ActorId { get; }

  Uri? BaseUrl { get; }

  ConfigurationAggregate Configuration { get; }
}
