using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Logitar.Portal.Core.Realms;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class PasswordRecoveryTemplateConfiguration : IEntityTypeConfiguration<PasswordRecoveryTemplate>
  {
    public void Configure(EntityTypeBuilder<PasswordRecoveryTemplate> builder)
    {
      builder.ToTable("RealmPasswordRecoveryTemplates");

      builder.HasKey(x => x.RealmSid);

      builder.HasOne(x => x.Realm).WithOne(x => x.PasswordRecoveryTemplateRelation)
        .HasForeignKey<PasswordRecoveryTemplate>(x => x.RealmSid);
    }
  }
}
