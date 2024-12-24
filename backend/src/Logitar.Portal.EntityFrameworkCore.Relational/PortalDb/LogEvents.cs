using Logitar.Data;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Portal.EntityFrameworkCore.Relational.PortalDb;

public static class LogEvents
{
  public static readonly TableId Table = new(nameof(PortalContext.LogEvents));

  public static readonly ColumnId EventId = new(nameof(LogEventEntity.EventId));
  public static readonly ColumnId LogId = new(nameof(LogEventEntity.LogId));
}
