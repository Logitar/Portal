using Logitar.Data;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

internal static class PortalDb
{
  public static class Actors
  {
    public static readonly TableId Table = new(nameof(PortalContext.Actors));
  }

  public static class Realms
  {
    public static readonly TableId Table = new(nameof(PortalContext.Realms));

    public static ColumnId AggregateId = new(nameof(RealmEntity.AggregateId), Table);
    public static ColumnId DisplayName = new(nameof(RealmEntity.DisplayName), Table);
    public static ColumnId UniqueSlug = new(nameof(RealmEntity.UniqueSlug), Table);
    public static ColumnId UniqueSlugNormalized = new(nameof(RealmEntity.UniqueSlugNormalized), Table);
  }
}
