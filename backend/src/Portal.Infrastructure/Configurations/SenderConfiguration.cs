using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portal.Core.Senders;

namespace Portal.Infrastructure.Configurations
{
  internal class SenderConfiguration : AggregateConfiguration<Sender>, IEntityTypeConfiguration<Sender>
  {
    public override void Configure(EntityTypeBuilder<Sender> builder)
    {
      base.Configure(builder);

      builder.HasIndex(x => x.DisplayName);
      builder.HasIndex(x => x.EmailAddress);
      builder.HasIndex(x => x.IsDefault);
      builder.HasIndex(x => x.Provider);

      builder.Ignore(x => x.Settings);

      builder.Property(x => x.DisplayName).HasMaxLength(256);
      builder.Property(x => x.EmailAddress).HasMaxLength(256);
      builder.Property(x => x.IsDefault).HasDefaultValue(false);
      builder.Property(x => x.Provider).HasDefaultValue(default(ProviderType));
      builder.Property(x => x.SettingsSerialized).HasColumnName(nameof(Sender.Settings)).HasColumnType("jsonb");
    }
  }
}
