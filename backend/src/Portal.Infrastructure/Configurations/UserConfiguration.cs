using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portal.Core.Users;

namespace Portal.Infrastructure.Configurations
{
  internal class UserConfiguration : AggregateConfiguration<User>, IEntityTypeConfiguration<User>
  {
    public override void Configure(EntityTypeBuilder<User> builder)
    {
      base.Configure(builder);

      builder.HasIndex(x => x.Email);
      builder.HasIndex(x => x.FirstName);
      builder.HasIndex(x => x.LastName);
      builder.HasIndex(x => x.MiddleName);
      builder.HasIndex(x => x.PhoneNumber);
      builder.HasIndex(x => x.Username);
      builder.HasIndex(x => new { x.RealmSid, x.UsernameNormalized }).IsUnique();

      builder.Property(x => x.Email).HasMaxLength(256);
      builder.Property(x => x.EmailNormalized).HasMaxLength(256);
      builder.Property(x => x.FirstName).HasMaxLength(128);
      builder.Property(x => x.LastName).HasMaxLength(128);
      builder.Property(x => x.Locale).HasMaxLength(16);
      builder.Property(x => x.MiddleName).HasMaxLength(128);
      builder.Property(x => x.Picture).HasMaxLength(2048);
      builder.Property(x => x.Username).HasMaxLength(256);
      builder.Property(x => x.UsernameNormalized).HasMaxLength(256);
    }
  }
}
