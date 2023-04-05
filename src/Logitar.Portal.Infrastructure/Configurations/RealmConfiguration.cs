using Logitar.Portal.Domain.Realms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class RealmConfiguration : AggregateConfiguration<Realm>, IEntityTypeConfiguration<Realm>
  {
    public override void Configure(EntityTypeBuilder<Realm> builder)
    {
      base.Configure(builder);

      builder.HasIndex(x => x.Alias);
      builder.HasIndex(x => x.AliasNormalized).IsUnique();
      builder.HasIndex(x => x.Name);

      builder.Ignore(x => x.PasswordRecoverySender);
      builder.Ignore(x => x.PasswordRecoveryTemplate);
      builder.Ignore(x => x.PasswordSettings);

      builder.Property(x => x.Alias).HasMaxLength(256);
      builder.Property(x => x.AliasNormalized).HasMaxLength(256);
      builder.Property(x => x.DefaultLocale).HasMaxLength(16);
      builder.Property(x => x.GoogleClientId).HasMaxLength(256);
      builder.Property(x => x.Name).HasMaxLength(256);
      builder.Property(x => x.PasswordSettingsSerialized).HasColumnName(nameof(Realm.PasswordSettings)).HasColumnType("jsonb");
      builder.Property(x => x.RequireConfirmedAccount).HasDefaultValue(false);
      builder.Property(x => x.RequireUniqueEmail).HasDefaultValue(false);
      builder.Property(x => x.Url).HasMaxLength(2048);
    }
  }
}
