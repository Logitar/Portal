using Logitar.EventSourcing;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class UserConfiguration : AggregateConfiguration<UserEntity>, IEntityTypeConfiguration<UserEntity>
{
  public override void Configure(EntityTypeBuilder<UserEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(PortalContext.Users));
    builder.HasKey(x => x.UserId);

    builder.HasIndex(x => x.TenantId);
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => x.HasPassword);
    builder.HasIndex(x => x.PasswordChangedOn);
    builder.HasIndex(x => x.DisabledOn);
    builder.HasIndex(x => x.AuthenticatedOn);
    builder.HasIndex(x => x.IsDisabled);
    builder.HasIndex(x => x.AddressFormatted);
    builder.HasIndex(x => x.EmailAddress);
    builder.HasIndex(x => x.PhoneE164Formatted);
    builder.HasIndex(x => x.IsConfirmed);
    builder.HasIndex(x => x.FirstName);
    builder.HasIndex(x => x.MiddleName);
    builder.HasIndex(x => x.LastName);
    builder.HasIndex(x => x.FullName);
    builder.HasIndex(x => x.Nickname);
    builder.HasIndex(x => x.Birthdate);
    builder.HasIndex(x => x.Gender);
    builder.HasIndex(x => x.Locale);
    builder.HasIndex(x => x.TimeZone);
    builder.HasIndex(x => new { x.TenantId, x.UniqueNameNormalized }).IsUnique();
    builder.HasIndex(x => new { x.TenantId, x.EmailAddressNormalized });

    builder.Ignore(x => x.CustomAttributes);

    builder.Property(x => x.TenantId).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.UniqueName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Password).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.PasswordChangedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.DisabledBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.AddressStreet).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.AddressLocality).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.AddressRegion).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.AddressPostalCode).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.AddressCountry).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.AddressFormatted).HasMaxLength(1536); // NOTE(fpion): 5 fields, with new lines (+255), and 4 separators
    builder.Property(x => x.AddressVerifiedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.EmailAddress).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.EmailAddressNormalized).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.EmailVerifiedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.PhoneCountryCode).HasMaxLength(16);
    builder.Property(x => x.PhoneNumber).HasMaxLength(32);
    builder.Property(x => x.PhoneExtension).HasMaxLength(16);
    builder.Property(x => x.PhoneE164Formatted).HasMaxLength(16);
    builder.Property(x => x.PhoneVerifiedBy).HasMaxLength(ActorId.MaximumLength);
    builder.Property(x => x.FirstName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.MiddleName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.LastName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.FullName).HasMaxLength(768); // NOTE(fpion): FirstName, MiddleName, LastName (3 * 255) + 2 space characters
    builder.Property(x => x.Nickname).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Gender).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Locale).HasMaxLength(16);
    builder.Property(x => x.TimeZone).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Picture).HasMaxLength(Constants.UrlMaximumLength);
    builder.Property(x => x.Profile).HasMaxLength(Constants.UrlMaximumLength);
    builder.Property(x => x.Website).HasMaxLength(Constants.UrlMaximumLength);
    builder.Property(x => x.CustomAttributesSerialized).HasColumnName(nameof(UserEntity.CustomAttributes));

    builder.HasMany(x => x.Roles).WithMany(x => x.Users).UsingEntity<UserRoleEntity>();
  }
}
