using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Configurations;

internal class UserConfiguration : AggregateConfiguration<UserEntity>, IEntityTypeConfiguration<UserEntity>
{
  public override void Configure(EntityTypeBuilder<UserEntity> builder)
  {
    base.Configure(builder);

    builder.HasKey(x => x.UserId);

    builder.HasIndex(x => x.Username);
    builder.HasIndex(x => x.PasswordChangedById);
    builder.HasIndex(x => x.PasswordChangedOn);
    builder.HasIndex(x => x.DisabledById);
    builder.HasIndex(x => x.DisabledOn);
    builder.HasIndex(x => x.IsDisabled);
    builder.HasIndex(x => x.SignedInOn);
    builder.HasIndex(x => x.AddressFormatted);
    builder.HasIndex(x => x.AddressVerifiedById);
    builder.HasIndex(x => x.EmailAddress);
    builder.HasIndex(x => x.EmailVerifiedById);
    builder.HasIndex(x => x.PhoneE164Formatted);
    builder.HasIndex(x => x.PhoneVerifiedById);
    builder.HasIndex(x => x.IsConfirmed);
    builder.HasIndex(x => x.FirstName);
    builder.HasIndex(x => x.MiddleName);
    builder.HasIndex(x => x.LastName);
    builder.HasIndex(x => x.FullName);
    builder.HasIndex(x => x.Nickname);
    builder.HasIndex(x => new { x.RealmId, x.UsernameNormalized }).IsUnique();
    builder.HasIndex(x => new { x.RealmId, x.EmailAddressNormalized });

    builder.Property(x => x.Username).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.UsernameNormalized).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.PasswordChangedBy).HasColumnType("jsonb");
    builder.Property(x => x.Password).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.DisabledBy).HasColumnType("jsonb");
    builder.Property(x => x.AddressLine1).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.AddressLine2).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.AddressLocality).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.AddressPostalCode).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.AddressCountry).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.AddressRegion).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.AddressFormatted).HasMaxLength(ushort.MaxValue);
    builder.Property(x => x.AddressVerifiedBy).HasColumnType("jsonb");
    builder.Property(x => x.EmailAddress).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.EmailAddressNormalized).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.EmailVerifiedBy).HasColumnType("jsonb");
    builder.Property(x => x.PhoneCountryCode).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.PhoneNumber).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.PhoneExtension).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.PhoneE164Formatted).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.PhoneVerifiedBy).HasColumnType("jsonb");
    builder.Property(x => x.FirstName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.MiddleName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.LastName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.FullName).HasMaxLength(ushort.MaxValue);
    builder.Property(x => x.Nickname).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Gender).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Locale).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.TimeZone).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Picture).HasMaxLength(ushort.MaxValue);
    builder.Property(x => x.Profile).HasMaxLength(ushort.MaxValue);
    builder.Property(x => x.Website).HasMaxLength(ushort.MaxValue);
    builder.Property(x => x.CustomAttributes).HasColumnType("jsonb");

    builder.HasOne(x => x.Realm).WithMany(x => x.Users).OnDelete(DeleteBehavior.Restrict);
  }
}
