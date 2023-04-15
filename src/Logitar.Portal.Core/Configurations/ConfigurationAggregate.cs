using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.Core.Configurations.Events;
using Logitar.Portal.Core.Configurations.Validators;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Security;
using System.Globalization;

namespace Logitar.Portal.Core.Configurations;

public class ConfigurationAggregate : AggregateRoot
{
  public ConfigurationAggregate(AggregateId id) : base(id)
  {
  }

  public ConfigurationAggregate(AggregateId actorId, CultureInfo defaultLocale, string? secret = null,
    ReadOnlyUsernameSettings? usernameSettings = null, ReadOnlyPasswordSettings? passwordSettings = null,
    ReadOnlyLoggingSettings? loggingSettings = null) : base(new AggregateId(Guid.Empty))
  {
    ConfigurationInitialized e = new()
    {
      ActorId = actorId,
      DefaultLocale = defaultLocale,
      Secret = secret?.CleanTrim() ?? SecurityHelper.GenerateSecret(),
      UsernameSettings = usernameSettings ?? new(),
      PasswordSettings = passwordSettings ?? new(),
      LoggingSettings = loggingSettings ?? new()
    };
    new ConfigurationInitializedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }

  public CultureInfo DefaultLocale { get; private set; } = null!;
  public string Secret { get; private set; } = string.Empty;

  public ReadOnlyUsernameSettings UsernameSettings { get; private set; } = new();
  public ReadOnlyPasswordSettings PasswordSettings { get; private set; } = new();

  public ReadOnlyLoggingSettings LoggingSettings { get; private set; } = new();

  protected virtual void Apply(ConfigurationInitialized e)
  {
    DefaultLocale = e.DefaultLocale;
    Secret = e.Secret;

    UsernameSettings = e.UsernameSettings;
    PasswordSettings = e.PasswordSettings;

    LoggingSettings = e.LoggingSettings;
  }
}
