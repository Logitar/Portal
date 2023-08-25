using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class RealmConfiguration : AggregateConfiguration<RealmEntity>, IEntityTypeConfiguration<RealmEntity>
{
  public override void Configure(EntityTypeBuilder<RealmEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(PortalContext.Realms));
    builder.HasKey(x => x.RealmId);

    builder.HasIndex(x => x.UniqueSlug);
    builder.HasIndex(x => x.UniqueSlugNormalized).IsUnique();
    builder.HasIndex(x => x.DisplayName);

    builder.Ignore(x => x.ClaimMappings);
    builder.Ignore(x => x.CustomAttributes);

    builder.Property(x => x.UniqueSlug).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.UniqueSlugNormalized).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.DisplayName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.DefaultLocale).HasMaxLength(16);
    builder.Property(x => x.Secret).HasMaxLength(64);
    builder.Property(x => x.Url).HasMaxLength(Constants.UrlMaximumLength);
    builder.Property(x => x.AllowedUniqueNameCharacters).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.PasswordStrategy).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.ClaimMappingsSerialized).HasColumnName(nameof(RealmEntity.ClaimMappings));
    builder.Property(x => x.CustomAttributesSerialized).HasColumnName(nameof(RealmEntity.CustomAttributes));
  }
}
