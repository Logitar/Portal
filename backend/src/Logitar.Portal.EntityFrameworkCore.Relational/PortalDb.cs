using Logitar.Data;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

internal static class PortalDb
{
  public static class Realms
  {
    public static readonly TableId Table = new(nameof(PortalContext.Realms));

    public static ColumnId AggregateId = new(nameof(RealmEntity.AggregateId), Table);
    public static ColumnId AllowedUniqueNameCharacters = new(nameof(RealmEntity.AllowedUniqueNameCharacters), Table);
    public static ColumnId CreatedBy = new(nameof(RealmEntity.CreatedBy), Table);
    public static ColumnId CreatedOn = new(nameof(RealmEntity.CreatedOn), Table);
    public static ColumnId CustomAttributes = new(nameof(RealmEntity.CustomAttributes), Table);
    public static ColumnId DefaultLocale = new(nameof(RealmEntity.DefaultLocale), Table);
    public static ColumnId Description = new(nameof(RealmEntity.Description), Table);
    public static ColumnId DisplayName = new(nameof(RealmEntity.DisplayName), Table);
    public static ColumnId PasswordsHashingStrategy = new(nameof(RealmEntity.PasswordsHashingStrategy), Table);
    public static ColumnId PasswordsRequireDigit = new(nameof(RealmEntity.PasswordsRequireDigit), Table);
    public static ColumnId PasswordsRequireLowercase = new(nameof(RealmEntity.PasswordsRequireLowercase), Table);
    public static ColumnId PasswordsRequireNonAlphanumeric = new(nameof(RealmEntity.PasswordsRequireNonAlphanumeric), Table);
    public static ColumnId PasswordsRequireUppercase = new(nameof(RealmEntity.PasswordsRequireUppercase), Table);
    public static ColumnId PasswordsRequiredLength = new(nameof(RealmEntity.PasswordsRequiredLength), Table);
    public static ColumnId PasswordsRequiredUniqueChars = new(nameof(RealmEntity.PasswordsRequiredUniqueChars), Table);
    public static ColumnId RequireUniqueEmail = new(nameof(RealmEntity.RequireUniqueEmail), Table);
    public static ColumnId Secret = new(nameof(RealmEntity.Secret), Table);
    public static ColumnId UniqueSlug = new(nameof(RealmEntity.UniqueSlug), Table);
    public static ColumnId UniqueSlugNormalized = new(nameof(RealmEntity.UniqueSlugNormalized), Table);
    public static ColumnId UpdatedBy = new(nameof(RealmEntity.UpdatedBy), Table);
    public static ColumnId UpdatedOn = new(nameof(RealmEntity.UpdatedOn), Table);
    public static ColumnId Url = new(nameof(RealmEntity.Url), Table);
    public static ColumnId Version = new(nameof(RealmEntity.Version), Table);
  }
}
