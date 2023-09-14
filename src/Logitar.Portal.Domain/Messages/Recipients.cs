using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Domain.Messages;

public record Recipients
{
  private readonly List<ReadOnlyRecipient> _bcc = new();
  private readonly List<ReadOnlyRecipient> _cc = new();
  private readonly List<ReadOnlyRecipient> _to = new();

  public Recipients()
  {
  }

  public Recipients(int capacity) : this()
  {
    _bcc = new(capacity);
    _cc = new(capacity);
    _to = new(capacity);
  }

  public Recipients(IEnumerable<ReadOnlyRecipient> recipients) : this(recipients.Count())
  {
    foreach (ReadOnlyRecipient recipient in recipients)
    {
      switch (recipient.Type)
      {
        case RecipientType.Bcc:
          _bcc.Add(recipient);
          break;
        case RecipientType.CC:
          _cc.Add(recipient);
          break;
        case RecipientType.To:
          _to.Add(recipient);
          break;
      }
    }
  }

  public IReadOnlyCollection<ReadOnlyRecipient> Bcc => _bcc.AsReadOnly();
  public IReadOnlyCollection<ReadOnlyRecipient> CC => _cc.AsReadOnly();
  public IReadOnlyCollection<ReadOnlyRecipient> To => _to.AsReadOnly();
}
