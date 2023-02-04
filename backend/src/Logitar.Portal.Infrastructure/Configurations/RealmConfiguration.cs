using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class RealmConfiguration : AggregateConfiguration<RealmEntity>, IEntityTypeConfiguration<RealmEntity>
  {
    public override void Configure(EntityTypeBuilder<RealmEntity> builder)
    {
      base.Configure(builder);

      builder.HasKey(x => x.RealmId);

      builder.HasOne(x => x.PasswordRecoverySender).WithOne(x => x.UsedAsPasswordRecoverySenderInRealm).OnDelete(DeleteBehavior.Restrict);
      builder.HasOne(x => x.PasswordRecoveryTemplate).WithOne(x => x.UsedAsPasswordRecoveryTemplateInRealm).OnDelete(DeleteBehavior.Restrict);

      builder.HasIndex(x => x.Alias);
      builder.HasIndex(x => x.AliasNormalized).IsUnique();
      builder.HasIndex(x => x.DisplayName);

      builder.Property(x => x.Alias).HasMaxLength(256);
      builder.Property(x => x.AliasNormalized).HasMaxLength(256);
      builder.Property(x => x.DisplayName).HasMaxLength(256);
      builder.Property(x => x.DefaultLocale).HasMaxLength(16);
      builder.Property(x => x.JwtSecret).HasMaxLength(256);
      builder.Property(x => x.Url).HasMaxLength(2048);
      builder.Property(x => x.UsernameSettings).HasColumnType("jsonb");
      builder.Property(x => x.PasswordSettings).HasColumnType("jsonb");
      builder.Property(x => x.GoogleClientId).HasMaxLength(256);
    }
  }
}
