namespace Logitar.Portal.Contracts.Messages;

public record SentMessages
{
  public List<Guid> Ids { get; set; }

  public SentMessages() : this([])
  {
  }

  public SentMessages(IEnumerable<Guid> ids)
  {
    Ids = ids.ToList();
  }
}
