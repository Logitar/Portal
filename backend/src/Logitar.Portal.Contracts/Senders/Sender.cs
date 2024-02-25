using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Contracts.Senders;

public class Sender : Aggregate
{
  public bool IsDefault { get; set; }

  public Email Email { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public SenderProvider Provider { get; set; }
  public MailgunSettings? Mailgun { get; set; }
  public SendGridSettings? SendGrid { get; set; }

  public Realm? Realm { get; set; }

  public Sender() : this(new Email())
  {
  }

  public Sender(Email email)
  {
    Email = email;
  }
}
