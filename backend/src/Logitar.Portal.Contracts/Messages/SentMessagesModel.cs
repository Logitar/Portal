namespace Logitar.Portal.Contracts.Messages
{
  public record SentMessagesModel
  {
    public IEnumerable<string> Success { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<string> Error { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<string> Unsent { get; set; } = Enumerable.Empty<string>();
  }
}
