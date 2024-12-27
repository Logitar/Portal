using Logitar.Data;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Portal.EntityFrameworkCore.Relational.PortalDb;

public static class Dictionaries
{
  public static readonly TableId Table = new(nameof(PortalContext.Dictionaries));

  public static readonly ColumnId CreatedBy = new(nameof(DictionaryEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(DictionaryEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(DictionaryEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(DictionaryEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(DictionaryEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(DictionaryEntity.Version), Table);

  public static readonly ColumnId DictionaryId = new(nameof(DictionaryEntity.DictionaryId), Table);
  public static readonly ColumnId EntityId = new(nameof(DictionaryEntity.EntityId), Table);
  public static readonly ColumnId Entries = new(nameof(DictionaryEntity.Entries), Table);
  public static readonly ColumnId EntryCount = new(nameof(DictionaryEntity.EntryCount), Table);
  public static readonly ColumnId Locale = new(nameof(DictionaryEntity.Locale), Table);
  public static readonly ColumnId TenantId = new(nameof(DictionaryEntity.TenantId), Table);
}
