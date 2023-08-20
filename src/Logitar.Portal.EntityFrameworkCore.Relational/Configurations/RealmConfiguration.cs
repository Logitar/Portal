using Logitar.Identity.EntityFrameworkCore.Relational.Configurations;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class RealmConfiguration : AggregateConfiguration<RealmEntity>, IEntityTypeConfiguration<RealmEntity>
{
  public override void Configure(EntityTypeBuilder<RealmEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(PortalDb.Realms.Table.Table!, PortalDb.Realms.Table.Schema);
    builder.HasKey(x => x.RealmId);

    builder.Ignore(x => x.UniqueNameSettings);
    builder.Ignore(x => x.PasswordSettings);
    builder.Ignore(x => x.ClaimMappings);
    builder.Ignore(x => x.CustomAttributes);

    builder.HasIndex(x => x.UniqueSlug);
    builder.HasIndex(x => x.UniqueSlugNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);

    builder.Property(x => x.UniqueSlug).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.UniqueSlugNormalized).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.DisplayName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.DefaultLocale).HasMaxLength(16);
    builder.Property(x => x.Secret).HasMaxLength(64);
    builder.Property(x => x.Url).HasMaxLength(2048);
    builder.Property(x => x.UniqueNameSettingsSerialized).HasColumnName(nameof(RealmEntity.UniqueNameSettings));
    builder.Property(x => x.PasswordSettingsSerialized).HasColumnName(nameof(RealmEntity.PasswordSettings));
    builder.Property(x => x.ClaimMappingsSerialized).HasColumnName(nameof(RealmEntity.ClaimMappings));
    builder.Property(x => x.CustomAttributesSerialized).HasColumnName(nameof(RealmEntity.CustomAttributes));
  }
}
