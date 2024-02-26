using Logitar.Identity.Domain.Shared;
using Logitar.Identity.EntityFrameworkCore.Relational.Configurations;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Settings;
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

    builder.Ignore(x => x.CustomAttributes);

    builder.Property(x => x.UniqueSlug).HasMaxLength(UniqueSlugUnit.MaximumLength);
    builder.Property(x => x.UniqueSlugNormalized).HasMaxLength(UniqueSlugUnit.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayNameUnit.MaximumLength);
    builder.Property(x => x.DefaultLocale).HasMaxLength(LocaleUnit.MaximumLength);
    builder.Property(x => x.Secret).HasMaxLength(JwtSecretUnit.MaximumLength);
    builder.Property(x => x.Url).HasMaxLength(UrlUnit.MaximumLength);
    builder.Property(x => x.AllowedUniqueNameCharacters).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.PasswordHashingStrategy).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.CustomAttributesSerialized).HasColumnName(nameof(RealmEntity.CustomAttributes));
  }
}
