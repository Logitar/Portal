namespace Logitar.Portal.Infrastructure.Settings;

internal record CachingSettings
{
  public TimeSpan? ActorLifetime { get; set; } = TimeSpan.FromMinutes(15);
}
