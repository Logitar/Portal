using Logitar.Portal.Core2.Configurations.Events;
using Logitar.Portal.Infrastructure2.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.Infrastructure2.Handlers.Configurations
{
  internal class ConfigurationInitializedEventHandler : INotificationHandler<ConfigurationInitializedEvent>
  {
    private readonly PortalContext _context;
    private readonly ILogger<ConfigurationInitializedEventHandler> _logger;

    public ConfigurationInitializedEventHandler(PortalContext context, ILogger<ConfigurationInitializedEventHandler> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task Handle(ConfigurationInitializedEvent notification, CancellationToken cancellationToken)
    {
      try
      {
        _context.Configurations.Add(new ConfigurationEntity(ConfigurationKeys.DefaultLocale, notification.DefaultLocale));
        _context.Configurations.Add(new ConfigurationEntity(ConfigurationKeys.JwtSecret, notification.JwtSecret));

        if (notification.UsernameSettings.AllowedCharacters != null)
        {
          _context.Configurations.Add(new ConfigurationEntity(ConfigurationKeys.UsernameSettings_AllowedCharacters, notification.UsernameSettings.AllowedCharacters));
        }

        _context.Configurations.Add(new ConfigurationEntity(ConfigurationKeys.PasswordSettings_RequiredLength, notification.PasswordSettings.RequiredLength));
        _context.Configurations.Add(new ConfigurationEntity(ConfigurationKeys.PasswordSettings_RequiredUniqueChars, notification.PasswordSettings.RequiredUniqueChars));
        _context.Configurations.Add(new ConfigurationEntity(ConfigurationKeys.PasswordSettings_RequireNonAlphanumeric, notification.PasswordSettings.RequireNonAlphanumeric));
        _context.Configurations.Add(new ConfigurationEntity(ConfigurationKeys.PasswordSettings_RequireLowercase, notification.PasswordSettings.RequireLowercase));
        _context.Configurations.Add(new ConfigurationEntity(ConfigurationKeys.PasswordSettings_RequireUppercase, notification.PasswordSettings.RequireUppercase));
        _context.Configurations.Add(new ConfigurationEntity(ConfigurationKeys.PasswordSettings_RequireDigit, notification.PasswordSettings.RequireDigit));

        await _context.SaveChangesAsync(cancellationToken);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "An unexpected error occurred.");
      }
    }
  }
}
