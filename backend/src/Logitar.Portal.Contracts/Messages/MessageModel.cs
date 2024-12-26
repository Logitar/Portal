using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Contracts.Templates;

namespace Logitar.Portal.Contracts.Messages;

public class MessageModel : AggregateModel
{
  public string Subject { get; set; }
  public ContentModel Body { get; set; }

  public int RecipientCount { get; set; }
  public List<RecipientModel> Recipients { get; set; }

  public SenderModel Sender { get; set; }
  public TemplateModel Template { get; set; }

  public bool IgnoreUserLocale { get; set; }
  public LocaleModel? Locale { get; set; }

  public List<Variable> Variables { get; set; }

  public bool IsDemo { get; set; }

  public MessageStatus Status { get; set; }
  public List<ResultData> ResultData { get; set; }

  public RealmModel? Realm { get; set; }

  public MessageModel() : this(string.Empty, new ContentModel(), new SenderModel(), new TemplateModel())
  {
  }

  public MessageModel(string subject, ContentModel body, SenderModel sender, TemplateModel template)
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
