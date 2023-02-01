using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Domain.Configurations.Events
{
  public record ConfigurationInitializedEvent : DomainEvent, INotification
  {
    public string DefaultLocaleName { get; init; } = string.Empty;
    public string JwtSecret { get; init; } = string.Empty;

    public UsernameSettings UsernameSettings { get; init; } = new();
    public PasswordSettings PasswordSettings { get; init; } = new();
  }
}
