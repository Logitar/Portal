using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portal.Core.Sessions;

namespace Portal.Infrastructure.Configurations
{
  internal class SessionConfiguration : AggregateConfiguration<Session>, IEntityTypeConfiguration<Session>
  {
    public override void Configure(EntityTypeBuilder<Session> builder)
    {
      base.Configure(builder);

      builder.Property(x => x.AdditionalInformation).HasColumnType("jsonb");
      builder.Property(x => x.IpAddress).HasMaxLength(40);
      builder.Property(x => x.IsActive).HasDefaultValue(false);
      builder.Property(x => x.IsPersistent).HasDefaultValue(false);
    }
  }
}
