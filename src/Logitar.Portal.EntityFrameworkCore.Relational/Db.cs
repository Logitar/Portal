using Logitar.Data;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

internal static class Db
{
  public static class ApiKeys
  {
    public static readonly TableId Table = new(nameof(PortalContext.ApiKeys));

    public static readonly ColumnId AggregateId = new(nameof(ApiKeyEntity.AggregateId), Table);
    public static readonly ColumnId ApiKeyId = new(nameof(ApiKeyEntity.ApiKeyId), Table);
    public static readonly ColumnId DisplayName = new(nameof(ApiKeyEntity.DisplayName), Table);
    public static readonly ColumnId ExpiresOn = new(nameof(ApiKeyEntity.ExpiresOn), Table);
    public static readonly ColumnId TenantId = new(nameof(ApiKeyEntity.TenantId), Table);
  }

  public static class ApiKeyRoles
  {
    public static readonly TableId Table = new(nameof(PortalContext.ApiKeyRoles));

    public static readonly ColumnId ApiKeyId = new(nameof(ApiKeyRoleEntity.ApiKeyId), Table);
    public static readonly ColumnId RoleId = new(nameof(ApiKeyRoleEntity.RoleId), Table);
  }

  public static class Dictionaries
  {
    public static readonly TableId Table = new(nameof(PortalContext.Dictionaries));

    public static readonly ColumnId AggregateId = new(nameof(DictionaryEntity.AggregateId), Table);
    public static readonly ColumnId Locale = new(nameof(DictionaryEntity.Locale), Table);
    public static readonly ColumnId TenantId = new(nameof(DictionaryEntity.TenantId), Table);
  }

  public static class Events
  {
    public static readonly TableId Table = new(nameof(EventContext.Events));

    public static readonly ColumnId AggregateId = new(nameof(EventEntity.AggregateId), Table);
    public static readonly ColumnId AggregateType = new(nameof(EventEntity.AggregateType), Table);
  }

  public static class Messages
  {
    public static readonly TableId Table = new(nameof(PortalContext.Messages));

    public static readonly ColumnId AggregateId = new(nameof(MessageEntity.AggregateId), Table);
    public static readonly ColumnId IsDemo = new(nameof(MessageEntity.IsDemo), Table);
    public static readonly ColumnId RealmId = new(nameof(MessageEntity.RealmId), Table);
    public static readonly ColumnId Status = new(nameof(MessageEntity.Status), Table);
    public static readonly ColumnId Subject = new(nameof(MessageEntity.Subject), Table);
    public static readonly ColumnId TemplateId = new(nameof(MessageEntity.TemplateId), Table);
  }

  public static class Realms
  {
    public static readonly TableId Table = new(nameof(PortalContext.Realms));

    public static readonly ColumnId AggregateId = new(nameof(RealmEntity.AggregateId), Table);
    public static readonly ColumnId DisplayName = new(nameof(RealmEntity.DisplayName), Table);
    public static readonly ColumnId RealmId = new(nameof(RealmEntity.RealmId), Table);
    public static readonly ColumnId UniqueSlug = new(nameof(RealmEntity.UniqueSlug), Table);
    public static readonly ColumnId UniqueSlugNormalized = new(nameof(RealmEntity.UniqueSlugNormalized), Table);
  }

  public static class Roles
  {
    public static readonly TableId Table = new(nameof(PortalContext.Roles));

    public static readonly ColumnId AggregateId = new(nameof(RoleEntity.AggregateId), Table);
    public static readonly ColumnId DisplayName = new(nameof(RoleEntity.DisplayName), Table);
    public static readonly ColumnId RoleId = new(nameof(RoleEntity.RoleId), Table);
    public static readonly ColumnId TenantId = new(nameof(RoleEntity.TenantId), Table);
    public static readonly ColumnId UniqueName = new(nameof(RoleEntity.UniqueName), Table);
    public static readonly ColumnId UniqueNameNormalized = new(nameof(RoleEntity.UniqueNameNormalized), Table);
  }

  public static class Senders
  {
    public static readonly TableId Table = new(nameof(PortalContext.Senders));

    public static readonly ColumnId AggregateId = new(nameof(SenderEntity.AggregateId), Table);
    public static readonly ColumnId DisplayName = new(nameof(SenderEntity.DisplayName), Table);
    public static readonly ColumnId EmailAddress = new(nameof(SenderEntity.EmailAddress), Table);
    public static readonly ColumnId IsDefault = new(nameof(SenderEntity.IsDefault), Table);
    public static readonly ColumnId Provider = new(nameof(SenderEntity.Provider), Table);
    public static readonly ColumnId TenantId = new(nameof(SenderEntity.TenantId), Table);
  }

  public static class Sessions
  {
    public static readonly TableId Table = new(nameof(PortalContext.Sessions));

    public static readonly ColumnId AggregateId = new(nameof(SessionEntity.AggregateId), Table);
    public static readonly ColumnId IsActive = new(nameof(SessionEntity.IsActive), Table);
    public static readonly ColumnId IsPersistent = new(nameof(SessionEntity.IsPersistent), Table);
    public static readonly ColumnId UserId = new(nameof(SessionEntity.UserId), Table);
  }

  public static class Templates
  {
    public static readonly TableId Table = new(nameof(PortalContext.Templates));

    public static readonly ColumnId AggregateId = new(nameof(TemplateEntity.AggregateId), Table);
    public static readonly ColumnId ContentType = new(nameof(TemplateEntity.ContentType), Table);
    public static readonly ColumnId DisplayName = new(nameof(TemplateEntity.DisplayName), Table);
    public static readonly ColumnId Subject = new(nameof(TemplateEntity.Subject), Table);
    public static readonly ColumnId TemplateId = new(nameof(TemplateEntity.TemplateId), Table);
    public static readonly ColumnId TenantId = new(nameof(TemplateEntity.TenantId), Table);
    public static readonly ColumnId UniqueName = new(nameof(TemplateEntity.UniqueName), Table);
    public static readonly ColumnId UniqueNameNormalized = new(nameof(TemplateEntity.UniqueNameNormalized), Table);
  }

  public static class Users
  {
    public static readonly TableId Table = new(nameof(PortalContext.Users));

    public static readonly ColumnId AddressFormatted = new(nameof(UserEntity.AddressFormatted), Table);
    public static readonly ColumnId AggregateId = new(nameof(UserEntity.AggregateId), Table);
    public static readonly ColumnId EmailAddress = new(nameof(UserEntity.EmailAddress), Table);
    public static readonly ColumnId EmailAddressNormalized = new(nameof(UserEntity.EmailAddressNormalized), Table);
    public static readonly ColumnId FirstName = new(nameof(UserEntity.FirstName), Table);
    public static readonly ColumnId FullName = new(nameof(UserEntity.FullName), Table);
    public static readonly ColumnId Gender = new(nameof(UserEntity.Gender), Table);
    public static readonly ColumnId HasPassword = new(nameof(UserEntity.HasPassword), Table);
    public static readonly ColumnId IsConfirmed = new(nameof(UserEntity.IsConfirmed), Table);
    public static readonly ColumnId IsDisabled = new(nameof(UserEntity.IsDisabled), Table);
    public static readonly ColumnId LastName = new(nameof(UserEntity.LastName), Table);
    public static readonly ColumnId Locale = new(nameof(UserEntity.Locale), Table);
    public static readonly ColumnId MiddleName = new(nameof(UserEntity.MiddleName), Table);
    public static readonly ColumnId Nickname = new(nameof(UserEntity.Nickname), Table);
    public static readonly ColumnId PhoneE164Formatted = new(nameof(UserEntity.PhoneE164Formatted), Table);
    public static readonly ColumnId TenantId = new(nameof(UserEntity.TenantId), Table);
    public static readonly ColumnId TimeZone = new(nameof(UserEntity.TimeZone), Table);
    public static readonly ColumnId UniqueName = new(nameof(UserEntity.UniqueName), Table);
    public static readonly ColumnId UniqueNameNormalized = new(nameof(UserEntity.UniqueNameNormalized), Table);
    public static readonly ColumnId UserId = new(nameof(UserEntity.UserId), Table);
  }

  public static class UserIdentifiers
  {
    public static readonly TableId Table = new(nameof(PortalContext.UserIdentifiers));

    public static readonly ColumnId Key = new(nameof(UserIdentifierEntity.Key), Table);
    public static readonly ColumnId UserId = new(nameof(UserIdentifierEntity.UserId), Table);
    public static readonly ColumnId ValueNormalized = new(nameof(UserIdentifierEntity.ValueNormalized), Table);
  }

  public static class UserRoles
  {
    public static readonly TableId Table = new(nameof(PortalContext.UserRoles));

    public static readonly ColumnId RoleId = new(nameof(UserRoleEntity.RoleId), Table);
    public static readonly ColumnId UserId = new(nameof(UserRoleEntity.UserId), Table);
  }
}
