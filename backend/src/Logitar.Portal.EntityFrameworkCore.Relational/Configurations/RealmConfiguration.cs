using Logitar.Identity.Core;
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

    builder.Property(x => x.UniqueSlug).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.UniqueSlugNormalized).HasMaxLength(Slug.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.DefaultLocale).HasMaxLength(Locale.MaximumLength);
    builder.Property(x => x.Secret).HasMaxLength(JwtSecret.MaximumLength);
    builder.Property(x => x.Url).HasMaxLength(Url.MaximumLength);
    builder.Property(x => x.AllowedUniqueNameCharacters).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.PasswordHashingStrategy).HasMaxLength(byte.MaxValue);
  }
}
