namespace Logitar.Portal.Contracts.Messages;

public record SentMessages
{
  public IEnumerable<Guid> Success { get; set; } = Enumerable.Empty<Guid>();
  public IEnumerable<Guid> Error { get; set; } = Enumerable.Empty<Guid>();
  public IEnumerable<Guid> Unsent { get; set; } = Enumerable.Empty<Guid>();
}
