using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Messages;

public record SearchMessagesPayload : SearchPayload
{
  public bool? IsDemo { get; set; }
  public MessageStatus? Status { get; set; }
  public string? Template { get; set; }

  public new List<MessageSortOption> Sort { get; set; } = [];
}
