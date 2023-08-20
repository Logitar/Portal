using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Actors;

internal static class ActorHelper
{
  public static IEnumerable<ActorId> GetIds(params AggregateEntity[] aggregates)
  {
    return aggregates.SelectMany(x => new[] { new ActorId(x.CreatedBy), new ActorId(x.UpdatedBy) })
      .Distinct();
  }
}
