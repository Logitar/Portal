using Logitar.Portal.Domain.Configurations.Events;
using Logitar.Portal.Domain.Users;
using System.Globalization;

namespace Logitar.Portal.Domain.Configurations
{
  public class Configuration : AggregateRoot
  {
    public Configuration(CultureInfo defaultLocale, string jwtSecret,
      LoggingSettings loggingSettings,
      UsernameSettings usernameSettings, PasswordSettings passwordSettings,
      AggregateId actorId) : base(AggregateId)
    {
      ApplyChange(new ConfigurationInitializedEvent
      {
        DefaultLocale = defaultLocale,
        JwtSecret = jwtSecret,
        LoggingSettings = loggingSettings,
        UsernameSettings = usernameSettings,
        PasswordSettings = passwordSettings
      }, actorId);
    }
    private Configuration() : base()
    {
    }

    public static AggregateId AggregateId => new("Portal");

    public CultureInfo DefaultLocale { get; private set; } = CultureInfo.InvariantCulture;
    public string JwtSecret { get; private set; } = string.Empty;

    public LoggingSettings LoggingSettings { get; private set; } = new();

    public UsernameSettings UsernameSettings { get; private set; } = new();
    public PasswordSettings PasswordSettings { get; private set; } = new();

    protected virtual void Apply(ConfigurationInitializedEvent @event)
    {
      DefaultLocale = @event.DefaultLocale;
      JwtSecret = @event.JwtSecret;

      LoggingSettings = @event.LoggingSettings;

      UsernameSettings = @event.UsernameSettings;
      PasswordSettings = @event.PasswordSettings;
    }
  }
}
