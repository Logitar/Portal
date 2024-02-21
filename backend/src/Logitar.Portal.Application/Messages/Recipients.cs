using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.Application.Messages;

internal record Recipients
{
  private readonly List<RecipientUnit> _to;
  public IReadOnlyCollection<RecipientUnit> To => _to.AsReadOnly();

  private readonly List<RecipientUnit> _cc;
  public IReadOnlyCollection<RecipientUnit> CC => _cc.AsReadOnly();

  private readonly List<RecipientUnit> _bcc;
  public IReadOnlyCollection<RecipientUnit> Bcc => _bcc.AsReadOnly();

  public Recipients(IEnumerable<RecipientUnit> recipients) : this(recipients.Count())
  {
    foreach (RecipientUnit recipient in recipients)
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

  private Recipients(int capacity)
  {
    _to = new(capacity);
    _cc = new(capacity);
    _bcc = new(capacity);
  }
}
