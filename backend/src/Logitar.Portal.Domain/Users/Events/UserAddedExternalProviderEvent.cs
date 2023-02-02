using MediatR;

namespace Logitar.Portal.Domain.Users.Events
{
  public record UserAddedExternalProviderEvent : DomainEvent, INotification
  {
    public string Key { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string? DisplayName { get; init; }
  }
}
