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

  public static class Logs
  {
    public static readonly TableId Table = new(nameof(PortalContext.Logs));

    public static readonly ColumnId CorrelationId = new(nameof(LogEntity.CorrelationId), Table);
    public static readonly ColumnId LogId = new(nameof(LogEntity.LogId), Table);
    public static readonly ColumnId UniqueId = new(nameof(LogEntity.UniqueId), Table);
    // TODO(fpion): complete
  }

  public static class LogEvents
  {
    public static readonly TableId Table = new(nameof(PortalContext.LogEvents));

    public static readonly ColumnId EventId = new(nameof(LogEventEntity.EventId), Table);
    public static readonly ColumnId LogId = new(nameof(LogEventEntity.LogId), Table);
  }

  public static class Messages
  {
    public static readonly TableId Table = new(nameof(PortalContext.Messages));

    public static readonly ColumnId AggregateId = new(nameof(MessageEntity.AggregateId), Table);
    public static readonly ColumnId BodyText = new(nameof(MessageEntity.BodyText), Table);
    public static readonly ColumnId BodyType = new(nameof(MessageEntity.BodyType), Table);
    public static readonly ColumnId CreatedBy = new(nameof(MessageEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(MessageEntity.CreatedOn), Table);
    public static readonly ColumnId IgnoreUserLocale = new(nameof(MessageEntity.IgnoreUserLocale), Table);
    public static readonly ColumnId IsDemo = new(nameof(MessageEntity.IsDemo), Table);
    public static readonly ColumnId Locale = new(nameof(MessageEntity.Locale), Table);
    public static readonly ColumnId MessageId = new(nameof(MessageEntity.MessageId), Table);
    public static readonly ColumnId ResultData = new(nameof(MessageEntity.ResultData), Table);
    public static readonly ColumnId SenderAddress = new(nameof(MessageEntity.SenderAddress), Table);
    public static readonly ColumnId SenderDisplayName = new(nameof(MessageEntity.SenderDisplayName), Table);
    public static readonly ColumnId SenderId = new(nameof(MessageEntity.SenderId), Table);
    public static readonly ColumnId SenderIsDefault = new(nameof(MessageEntity.SenderIsDefault), Table);
    public static readonly ColumnId SenderProvider = new(nameof(MessageEntity.SenderProvider), Table);
    public static readonly ColumnId Status = new(nameof(MessageEntity.Status), Table);
    public static readonly ColumnId Subject = new(nameof(MessageEntity.Subject), Table);
    public static readonly ColumnId TemplateDisplayName = new(nameof(MessageEntity.TemplateDisplayName), Table);
    public static readonly ColumnId TemplateId = new(nameof(MessageEntity.TemplateId), Table);
    public static readonly ColumnId TemplateUniqueKey = new(nameof(MessageEntity.TemplateUniqueKey), Table);
    public static readonly ColumnId TenantId = new(nameof(MessageEntity.TenantId), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(MessageEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(MessageEntity.UpdatedOn), Table);
    public static readonly ColumnId Variables = new(nameof(MessageEntity.Variables), Table);
    public static readonly ColumnId Version = new(nameof(MessageEntity.Version), Table);
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

  public static class Senders
  {
    public static readonly TableId Table = new(nameof(PortalContext.Senders));

    public static readonly ColumnId AggregateId = new(nameof(SenderEntity.AggregateId), Table);
    public static readonly ColumnId CreatedBy = new(nameof(SenderEntity.CreatedBy), Table);
    public static readonly ColumnId CreatedOn = new(nameof(SenderEntity.CreatedOn), Table);
    public static readonly ColumnId Description = new(nameof(SenderEntity.Description), Table);
    public static readonly ColumnId DisplayName = new(nameof(SenderEntity.DisplayName), Table);
    public static readonly ColumnId EmailAddress = new(nameof(SenderEntity.EmailAddress), Table);
    public static readonly ColumnId IsDefault = new(nameof(SenderEntity.IsDefault), Table);
    public static readonly ColumnId Provider = new(nameof(SenderEntity.Provider), Table);
    public static readonly ColumnId SenderId = new(nameof(SenderEntity.SenderId), Table);
    public static readonly ColumnId Settings = new(nameof(SenderEntity.Settings), Table);
    public static readonly ColumnId TenantId = new(nameof(SenderEntity.TenantId), Table);
    public static readonly ColumnId UpdatedBy = new(nameof(SenderEntity.UpdatedBy), Table);
    public static readonly ColumnId UpdatedOn = new(nameof(SenderEntity.UpdatedOn), Table);
    public static readonly ColumnId Version = new(nameof(SenderEntity.Version), Table);
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
