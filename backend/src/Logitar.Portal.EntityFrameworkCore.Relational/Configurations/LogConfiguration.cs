using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class LogConfiguration : IEntityTypeConfiguration<LogEntity>
{
  public void Configure(EntityTypeBuilder<LogEntity> builder)
  {
    builder.ToTable(nameof(PortalContext.Logs));
    builder.HasKey(x => x.LogId);

    builder.HasIndex(x => x.UniqueId).IsUnique();
    builder.HasIndex(x => x.CorrelationId);
    builder.HasIndex(x => x.OperationType);
    builder.HasIndex(x => x.OperationName);
    builder.HasIndex(x => x.ActivityType);
    builder.HasIndex(x => x.StatusCode);
    builder.HasIndex(x => x.IsCompleted);
    builder.HasIndex(x => x.Level);
    builder.HasIndex(x => x.HasErrors);
    builder.HasIndex(x => x.StartedOn);
    builder.HasIndex(x => x.EndedOn);
    builder.HasIndex(x => x.Duration);
    builder.HasIndex(x => x.TenantId);
    builder.HasIndex(x => x.ActorId);
    builder.HasIndex(x => x.ApiKeyId);
    builder.HasIndex(x => x.UserId);
    builder.HasIndex(x => x.SessionId);

    builder.Property(x => x.CorrelationId).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Method).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Destination).HasMaxLength(Url.MaximumLength);
    builder.Property(x => x.Source).HasMaxLength(Url.MaximumLength);
    builder.Property(x => x.OperationType).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.OperationName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.ActivityType).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Level).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<LogLevel>());
    builder.Property(x => x.ActorId).HasMaxLength(ActorId.MaximumLength);
  }
}
