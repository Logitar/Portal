using Logitar.Portal.Core2.Users;
using MediatR;

namespace Logitar.Portal.Core2.Configurations.Events
{
  public class ConfigurationInitializedEvent : DomainEvent, INotification
  {
    public string DefaultLocale { get; init; } = null!;
    public string JwtSecret { get; init; } = null!;

    public UsernameSettings UsernameSettings { get; init; } = null!;
    public PasswordSettings PasswordSettings { get; init; } = null!;
  }
}
