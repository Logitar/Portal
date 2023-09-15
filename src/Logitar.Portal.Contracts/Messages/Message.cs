using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Contracts.Templates;

namespace Logitar.Portal.Contracts.Messages;

public record Message
{
  public Guid Id { get; set; }

  public string Subject { get; set; } = string.Empty;
  public string Body { get; set; } = string.Empty;

  public IEnumerable<Recipient> Recipients { get; set; } = Enumerable.Empty<Recipient>();
  public int RecipientCount { get; set; }

  public Realm? Realm { get; set; }
  public Sender Sender { get; set; } = new();
  public Template Template { get; set; } = new();

  public bool IgnoreUserLocale { get; set; }
  public string? Locale { get; set; }

  public IEnumerable<Variable> Variables { get; set; } = Enumerable.Empty<Variable>();

  public bool IsDemo { get; set; }

  public IEnumerable<ResultData> Result { get; set; } = Enumerable.Empty<ResultData>();
  public MessageStatus Status { get; set; }
}
