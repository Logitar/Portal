using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Contracts.Templates;

namespace Logitar.Portal.Contracts.Messages;

public class Message : Aggregate
{
  public string Subject { get; set; }
  public Content Body { get; set; }

  public int RecipientCount { get; set; }
  public List<Recipient> Recipients { get; set; }

  public Sender Sender { get; set; }
  public Template Template { get; set; }

  public bool IgnoreUserLocale { get; set; }
  public Locale? Locale { get; set; }

  public List<Variable> Variables { get; set; }

  public bool IsDemo { get; set; }

  public MessageStatus Status { get; set; }
  public List<ResultData> ResultData { get; set; }

  public Realm? Realm { get; set; }

  public Message() : this(string.Empty, new Content(), new Sender(), new Template())
  {
  }

  public Message(string subject, Content body, Sender sender, Template template)
  {
    Subject = subject;
    Body = body;
    Recipients = [];
    Sender = sender;
    Template = template;
    Variables = [];
    ResultData = [];
  }
}
