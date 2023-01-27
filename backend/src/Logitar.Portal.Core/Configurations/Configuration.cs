using Logitar.Portal.Core.Configurations.Events;
using Logitar.Portal.Core.Users;
using System.Globalization;

namespace Logitar.Portal.Core.Configurations
{
  public class Configuration : AggregateRoot
  {
    public Configuration(CultureInfo defaultLocale, string jwtSecret,
      UsernameSettings usernameSettings, PasswordSettings passwordSettings,
      AggregateId userId) : base(AggregateId)
    {
      ApplyChange(new ConfigurationInitializedEvent
      {
        DefaultLocale = defaultLocale.Name,
        JwtSecret = jwtSecret,
        UsernameSettings = usernameSettings,
        PasswordSettings = passwordSettings
      }, userId);
    }

    public static AggregateId AggregateId => new(nameof(Portal));

    public CultureInfo DefaultLocale { get; private set; } = null!;
    public string JwtSecret { get; private set; } = null!;

    public UsernameSettings UsernameSettings { get; private set; } = null!;
    public PasswordSettings PasswordSettings { get; private set; } = null!;

    protected virtual void Apply(ConfigurationInitializedEvent @event)
    {
      DefaultLocale = CultureInfo.GetCultureInfo(@event.DefaultLocale);
      JwtSecret = @event.JwtSecret;

      UsernameSettings = @event.UsernameSettings;
      PasswordSettings = @event.PasswordSettings;
    }
  }
}
