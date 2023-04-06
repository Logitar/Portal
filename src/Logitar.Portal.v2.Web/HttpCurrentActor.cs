using Logitar.EventSourcing;
using Logitar.Portal.v2.Core;

namespace Logitar.Portal.v2.Web;

internal class HttpCurrentActor : ICurrentActor
{
  /// <summary>
  /// TODO(fpion): Authentication
  /// </summary>
  public AggregateId Id => new(Guid.Empty);
}
