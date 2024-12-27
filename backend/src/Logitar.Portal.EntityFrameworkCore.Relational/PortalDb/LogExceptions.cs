using Logitar.Data;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Portal.EntityFrameworkCore.Relational.PortalDb;

public static class LogExceptions
{
  public static readonly TableId Table = new(nameof(PortalContext.LogExceptions));

  public static readonly ColumnId Data = new(nameof(LogExceptionEntity.Data));
  public static readonly ColumnId HResult = new(nameof(LogExceptionEntity.HResult));
  public static readonly ColumnId HelpLink = new(nameof(LogExceptionEntity.HelpLink));
  public static readonly ColumnId LogExceptionId = new(nameof(LogExceptionEntity.LogExceptionId));
  public static readonly ColumnId LogId = new(nameof(LogExceptionEntity.LogId));
  public static readonly ColumnId Message = new(nameof(LogExceptionEntity.Message));
  public static readonly ColumnId Source = new(nameof(LogExceptionEntity.Source));
  public static readonly ColumnId StackTrace = new(nameof(LogExceptionEntity.StackTrace));
  public static readonly ColumnId TargetSite = new(nameof(LogExceptionEntity.TargetSite));
  public static readonly ColumnId Type = new(nameof(LogExceptionEntity.Type));
}
