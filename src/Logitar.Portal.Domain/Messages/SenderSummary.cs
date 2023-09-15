using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Domain.Messages;

public record SenderSummary
{
  public AggregateId Id { get; init; }
  public bool IsDefault { get; init; }
  public ProviderType Provider { get; init; }
  public string Address { get; init; } = string.Empty;
  public string? DisplayName { get; init; }

  public static SenderSummary From(SenderAggregate sender) => new()
  {
    Id = sender.Id,
    IsDefault = sender.IsDefault,
    Provider = sender.Provider,
    Address = sender.EmailAddress,
    DisplayName = sender.DisplayName
  };
}
