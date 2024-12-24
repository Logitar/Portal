using Logitar.Data;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Portal.EntityFrameworkCore.Relational.PortalDb;

public static class Logs
{
  public static readonly TableId Table = new(nameof(PortalContext.Logs));

  public static readonly ColumnId ActivityData = new(nameof(LogEntity.ActivityData));
  public static readonly ColumnId ActivityType = new(nameof(LogEntity.ActivityType));
  public static readonly ColumnId ActorId = new(nameof(LogEntity.ActorId));
  public static readonly ColumnId AdditionalInformation = new(nameof(LogEntity.AdditionalInformation));
  public static readonly ColumnId ApiKeyId = new(nameof(LogEntity.ApiKeyId));
  public static readonly ColumnId CorrelationId = new(nameof(LogEntity.CorrelationId));
  public static readonly ColumnId Destination = new(nameof(LogEntity.Destination));
  public static readonly ColumnId Duration = new(nameof(LogEntity.Duration));
  public static readonly ColumnId EndedOn = new(nameof(LogEntity.EndedOn));
  public static readonly ColumnId HasErrors = new(nameof(LogEntity.HasErrors));
  public static readonly ColumnId IsCompleted = new(nameof(LogEntity.IsCompleted));
  public static readonly ColumnId Level = new(nameof(LogEntity.Level));
  public static readonly ColumnId LogId = new(nameof(LogEntity.LogId));
  public static readonly ColumnId Method = new(nameof(LogEntity.Method));
  public static readonly ColumnId OperationName = new(nameof(LogEntity.OperationName));
  public static readonly ColumnId OperationType = new(nameof(LogEntity.OperationType));
  public static readonly ColumnId SessionId = new(nameof(LogEntity.SessionId));
  public static readonly ColumnId Source = new(nameof(LogEntity.Source));
  public static readonly ColumnId StartedOn = new(nameof(LogEntity.StartedOn));
  public static readonly ColumnId StatusCode = new(nameof(LogEntity.StatusCode));
  public static readonly ColumnId TenantId = new(nameof(LogEntity.TenantId));
  public static readonly ColumnId UniqueId = new(nameof(LogEntity.UniqueId));
  public static readonly ColumnId UserId = new(nameof(LogEntity.UserId));
}
