using Logitar.Data;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Portal.EntityFrameworkCore.Relational.PortalDb;

public static class Recipients
{
  public static readonly TableId Table = new(nameof(PortalContext.Recipients));

  public static readonly ColumnId Address = new(nameof(RecipientEntity.Address), Table);
  public static readonly ColumnId DisplayName = new(nameof(RecipientEntity.DisplayName), Table);
  public static readonly ColumnId MessageId = new(nameof(RecipientEntity.MessageId), Table);
  public static readonly ColumnId RecipientId = new(nameof(RecipientEntity.RecipientId), Table);
  public static readonly ColumnId Type = new(nameof(RecipientEntity.Type), Table);
  public static readonly ColumnId UserId = new(nameof(RecipientEntity.UserId), Table);
}
