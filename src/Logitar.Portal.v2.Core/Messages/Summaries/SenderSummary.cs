using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Senders;
using Logitar.Portal.v2.Core.Senders;

namespace Logitar.Portal.v2.Core.Messages.Summaries;

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
