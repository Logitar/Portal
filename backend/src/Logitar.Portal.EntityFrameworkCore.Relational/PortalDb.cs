using Logitar.Data;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

internal static class PortalDb
{
  public static class Dictionaries
  {
    public static readonly TableId Table = new(nameof(PortalContext.Dictionaries));

    public static readonly ColumnId AggregateId = new(nameof(DictionaryEntity.AggregateId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(DictionaryEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(DictionaryEntity.CreatedOn), Table);
    public static readonly ColumnId DictionaryId = new(nameof(DictionaryEntity.DictionaryId), Table);
    public static readonly ColumnId Entries = new(nameof(DictionaryEntity.Entries), Table);
    public static readonly ColumnId EntryCount = new(nameof(DictionaryEntity.EntryCount), Table);
    public static readonly ColumnId Locale = new(nameof(DictionaryEntity.Locale), Table);
    public static readonly ColumnId TenantId = new(nameof(DictionaryEntity.TenantId), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(DictionaryEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(DictionaryEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(DictionaryEntity.Version), Table);
  }

  public static class Realms
  {
    public static readonly TableId Table = new(nameof(PortalContext.Realms));

    public static readonly ColumnId AggregateId = new(nameof(RealmEntity.AggregateId), Table);
    public static readonly ColumnId AllowedUniqueNameCharacters = new(nameof(RealmEntity.AllowedUniqueNameCharacters), Table);
    public static readonly ColumnId CreatedBy = new(nameof(RealmEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(RealmEntity.CreatedOn), Table);
    public static readonly ColumnId CustomAttributes = new(nameof(RealmEntity.CustomAttributes), Table);
    public static readonly ColumnId DefaultLocale = new(nameof(RealmEntity.DefaultLocale), Table);
    public static readonly ColumnId Description = new(nameof(RealmEntity.Description), Table);
    public static readonly ColumnId DisplayName = new(nameof(RealmEntity.DisplayName), Table);
    public static readonly ColumnId PasswordHashingStrategy = new(nameof(RealmEntity.PasswordHashingStrategy), Table);
    public static readonly ColumnId PasswordsRequireDigit = new(nameof(RealmEntity.PasswordsRequireDigit), Table);
    public static readonly ColumnId PasswordsRequireLowercase = new(nameof(RealmEntity.PasswordsRequireLowercase), Table);
    public static readonly ColumnId PasswordsRequireNonAlphanumeric = new(nameof(RealmEntity.PasswordsRequireNonAlphanumeric), Table);
    public static readonly ColumnId PasswordsRequireUppercase = new(nameof(RealmEntity.PasswordsRequireUppercase), Table);
    public static readonly ColumnId RealmId = new(nameof(RealmEntity.RealmId), Table);
    public static readonly ColumnId RequireUniqueEmail = new(nameof(RealmEntity.RequireUniqueEmail), Table);
    public static readonly ColumnId RequiredPasswordLength = new(nameof(RealmEntity.RequiredPasswordLength), Table);
    public static readonly ColumnId RequiredPasswordUniqueChars = new(nameof(RealmEntity.RequiredPasswordUniqueChars), Table);
    public static readonly ColumnId Secret = new(nameof(RealmEntity.Secret), Table);
    public static readonly ColumnId UniqueSlug = new(nameof(RealmEntity.UniqueSlug), Table);
    public static readonly ColumnId UniqueSlugNormalized = new(nameof(RealmEntity.UniqueSlugNormalized), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(RealmEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(RealmEntity.UpdatedOn), Table);
    public static readonly ColumnId Url = new(nameof(RealmEntity.Url), Table);
    public static readonly ColumnId Version = new(nameof(RealmEntity.Version), Table);
  }

  public static class Templates
  {
    public static readonly TableId Table = new(nameof(PortalContext.Templates));

    public static readonly ColumnId AggregateId = new(nameof(TemplateEntity.AggregateId), Table);
    public static readonly ColumnId ContentText = new(nameof(TemplateEntity.ContentText), Table);
    public static readonly ColumnId ContentType = new(nameof(TemplateEntity.ContentType), Table);
    public static readonly ColumnId CreatedBy = new(nameof(TemplateEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(TemplateEntity.CreatedOn), Table);
    public static readonly ColumnId Description = new(nameof(TemplateEntity.Description), Table);
    public static readonly ColumnId DisplayName = new(nameof(TemplateEntity.DisplayName), Table);
    public static readonly ColumnId Subject = new(nameof(TemplateEntity.Subject), Table);
    public static readonly ColumnId TemplateId = new(nameof(TemplateEntity.TemplateId), Table);
    public static readonly ColumnId TenantId = new(nameof(TemplateEntity.TenantId), Table);
    public static readonly ColumnId UniqueKey = new(nameof(TemplateEntity.UniqueKey), Table);
    public static readonly ColumnId UniqueKeyNormalized = new(nameof(TemplateEntity.UniqueKeyNormalized), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(TemplateEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(TemplateEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(TemplateEntity.Version), Table);
  }
}
