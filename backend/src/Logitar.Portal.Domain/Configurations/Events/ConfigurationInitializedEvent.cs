using Logitar.Portal.Domain.Users;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Domain.Configurations.Events
{
  public record ConfigurationInitializedEvent : DomainEvent, INotification
  {
    public CultureInfo DefaultLocale { get; init; } = CultureInfo.InvariantCulture;
    public string JwtSecret { get; init; } = string.Empty;

    public UsernameSettings UsernameSettings { get; init; } = new();
    public PasswordSettings PasswordSettings { get; init; } = new();
  }
}
