using Logitar.Data;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

internal static class Db
{
  public static class Events
  {
    public static readonly TableId Table = new(nameof(EventContext.Events));

    public static readonly ColumnId AggregateId = new(nameof(EventEntity.AggregateId), Table);
    public static readonly ColumnId AggregateType = new(nameof(EventEntity.AggregateType), Table);
  }

  public static class Realms
  {
    public static readonly TableId Table = new(nameof(PortalContext.Realms));

    public static readonly ColumnId AggregateId = new(nameof(RealmEntity.AggregateId), Table);
    public static readonly ColumnId UniqueSlugNormalized = new(nameof(RealmEntity.UniqueSlugNormalized), Table);
  }

  public static class Sessions
  {
    public static readonly TableId Table = new(nameof(PortalContext.Sessions));

    public static readonly ColumnId AggregateId = new(nameof(SessionEntity.AggregateId), Table);
    public static readonly ColumnId IsActive = new(nameof(SessionEntity.IsActive), Table);
    public static readonly ColumnId IsPersistent = new(nameof(SessionEntity.IsPersistent), Table);
    public static readonly ColumnId UserId = new(nameof(SessionEntity.UserId), Table);
  }

  public static class Users
  {
    public static readonly TableId Table = new(nameof(PortalContext.Users));

    public static readonly ColumnId AggregateId = new(nameof(UserEntity.AggregateId), Table);
    public static readonly ColumnId EmailAddressNormalized = new(nameof(UserEntity.EmailAddressNormalized), Table);
    public static readonly ColumnId TenantId = new(nameof(UserEntity.TenantId), Table);
    public static readonly ColumnId UniqueNameNormalized = new(nameof(UserEntity.UniqueNameNormalized), Table);
    public static readonly ColumnId UserId = new(nameof(UserEntity.UserId), Table);
  }
}
