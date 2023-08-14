using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.Domain.Configurations.Events;
using Logitar.Portal.Domain.Configurations.Validators;
using Logitar.Security.Cryptography;

namespace Logitar.Portal.Domain.Configurations;

public class ConfigurationAggregate : AggregateRoot
{
  public ConfigurationAggregate(AggregateId id) : base(id)
  {
  }

  public ConfigurationAggregate(CultureInfo defaultLocale, ActorId actorId)
    : base(new AggregateId("CONFIGURATION"))
  {
    ConfigurationInitializedEvent initialized = new()
    {
      ActorId = actorId,
      DefaultLocale = defaultLocale,
      Secret = RandomStringGenerator.GetString()
    };
    new ConfigurationInitializedValidator().ValidateAndThrow(initialized);

    ApplyChange(initialized);
  }
  protected virtual void Apply(ConfigurationInitializedEvent initialized)
  {
    DefaultLocale = initialized.DefaultLocale;
    Secret = initialized.Secret;

    UniqueNameSettings = initialized.UniqueNameSettings;
    PasswordSettings = initialized.PasswordSettings;

    LoggingSettings = initialized.LoggingSettings;
  }

  public CultureInfo DefaultLocale { get; private set; } = CultureInfo.InvariantCulture;
  public string Secret { get; private set; } = string.Empty;

  public ReadOnlyUniqueNameSettings UniqueNameSettings { get; private set; } = new();
  public ReadOnlyPasswordSettings PasswordSettings { get; private set; } = new();

  public ReadOnlyLoggingSettings LoggingSettings { get; private set; } = new();
}
