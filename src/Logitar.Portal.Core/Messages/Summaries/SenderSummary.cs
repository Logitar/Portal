using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Core.Senders;

namespace Logitar.Portal.Core.Messages.Summaries;

public record SenderSummary
{
  public AggregateId Id { get; init; }

  public ProviderType Provider { get; init; }

  public bool IsDefault { get; init; }

  public string EmailAddress { get; init; } = string.Empty;
  public string? DisplayName { get; init; }

  public static SenderSummary From(SenderAggregate sender) => new()
  {
    Id = sender.Id,
    Provider = sender.Provider,
    IsDefault = sender.IsDefault,
    EmailAddress = sender.EmailAddress,
    DisplayName = sender.DisplayName
  };
}
