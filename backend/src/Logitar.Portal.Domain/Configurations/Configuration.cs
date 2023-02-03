using Logitar.Portal.Domain.Configurations.Events;
using Logitar.Portal.Domain.Users;
using System.Globalization;

namespace Logitar.Portal.Domain.Configurations
{
  public class Configuration : AggregateRoot
  {
    public Configuration(CultureInfo defaultLocale, string jwtSecret,
      UsernameSettings usernameSettings, PasswordSettings passwordSettings,
      AggregateId userId) : base(AggregateId)
    {
      ApplyChange(new ConfigurationInitializedEvent
      {
        DefaultLocale = defaultLocale,
        JwtSecret = jwtSecret,
        UsernameSettings = usernameSettings,
        PasswordSettings = passwordSettings
      }, userId);
    }
    private Configuration() : base()
    {
    }

    public static AggregateId AggregateId => new(nameof(Portal));

    public CultureInfo DefaultLocale { get; private set; } = CultureInfo.InvariantCulture;
    public string JwtSecret { get; private set; } = string.Empty;

    public UsernameSettings UsernameSettings { get; private set; } = new();
    public PasswordSettings PasswordSettings { get; private set; } = new();

    protected virtual void Apply(ConfigurationInitializedEvent @event)
    {
      DefaultLocale = @event.DefaultLocale;
      JwtSecret = @event.JwtSecret;

      UsernameSettings = @event.UsernameSettings;
      PasswordSettings = @event.PasswordSettings;
    }
  }
}
