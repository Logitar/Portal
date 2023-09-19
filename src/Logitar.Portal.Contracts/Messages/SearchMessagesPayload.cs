namespace Logitar.Portal.Contracts.Messages;

public record SearchMessagesPayload : SearchPayload
{
  public string? Realm { get; set; }

  public bool? IsDemo { get; set; }
  public MessageStatus? Status { get; set; }
  public string? Template { get; set; }

  public new IEnumerable<MessageSortOption> Sort { get; set; } = Enumerable.Empty<MessageSortOption>();
}
