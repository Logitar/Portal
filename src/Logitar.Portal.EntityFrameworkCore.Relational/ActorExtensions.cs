using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

internal static class ActorExtensions
{
  public static ISet<string> GetActorIds(this AggregateEntity entity)
  {
    return new[] { entity.CreatedBy, entity.UpdatedBy }.ToHashSet();
  }
}
