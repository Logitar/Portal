namespace Logitar.Portal.Contracts.Messages;

public record SentMessages
{
  public SentMessages() : this(Enumerable.Empty<Guid>())
  {
  }
  public SentMessages(IEnumerable<Guid> ids)
  {
    Ids = ids;
  }

  public IEnumerable<Guid> Ids { get; set; }
}
