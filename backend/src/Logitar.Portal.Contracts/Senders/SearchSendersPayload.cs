using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Senders;

public record SearchSendersPayload : SearchPayload
{
  public SenderProvider? Provider { get; set; }

  public new List<SenderSortOption> Sort { get; set; } = [];
}
