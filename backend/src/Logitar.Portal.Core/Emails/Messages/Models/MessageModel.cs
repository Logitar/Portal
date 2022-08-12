using Logitar.Portal.Core.Emails.Senders;

namespace Logitar.Portal.Core.Emails.Messages.Models
{
  public class MessageModel : AggregateModel
  {
    public bool IsDemo { get; set; }

    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;

    public IEnumerable<RecipientModel> Recipients { get; set; } = null!;

    public Guid SenderId { get; set; }
    public bool SenderIsDefault { get; set; }
    public ProviderType SenderProvider { get; set; }
    public string SenderAddress { get; set; } = null!;
    public string? SenderDisplayName { get; set; }

    public Guid? RealmId { get; set; }
    public string? RealmAlias { get; set; }
    public string? RealmName { get; set; }

    public Guid TemplateId { get; set; }
    public string TemplateKey { get; set; } = null!;
    public string TemplateContentType { get; set; } = null!;
    public string? TemplateDisplayName { get; set; }

    public bool IgnoreUserLocale { get; private set; }
    public string? Locale { get; private set; }

    public IEnumerable<VariableModel> Variables { get; set; } = null!;

    public IEnumerable<ErrorModel> Errors { get; set; } = null!;
    public bool HasErrors => Errors.Any();

    public IEnumerable<ResultDataModel> Result { get; set; } = null!;
    public bool Succeeded => !HasErrors && Result != null;
  }
}
