namespace Logitar.Portal.Core.Emails.Messages.Models
{
  public class SentMessagesModel
  {
    public IEnumerable<Guid> Success { get; set; } = Enumerable.Empty<Guid>();
    public IEnumerable<Guid> Error { get; set; } = Enumerable.Empty<Guid>();
    public IEnumerable<Guid> Unsent { get; set; } = Enumerable.Empty<Guid>();
  }
}
