using Logitar.Portal.Contracts.Senders;
using System.Globalization;

namespace Logitar.Portal.Contracts.Messages
{
  public record MessageModel : AggregateModel
  {
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    public IEnumerable<RecipientModel> Recipients { get; set; } = Enumerable.Empty<RecipientModel>();
    public int RecipientCount { get; set; }

    public string SenderId { get; set; } = string.Empty;
    public bool SenderIsDefault { get; set; }
    public string SenderAddress { get; set; } = string.Empty;
    public string? SenderDisplayName { get; set; }
    public ProviderType SenderProvider { get; set; }

    public string TemplateId { get; set; } = string.Empty;
    public string TemplateKey { get; set; } = string.Empty;
    public string? TemplateDisplayName { get; set; }
    public string TemplateContentType { get; set; } = string.Empty;

    public string? RealmId { get; set; }
    public string? RealmAlias { get; set; }
    public string? RealmDisplayName { get; set; }

    public bool IgnoreUserLocale { get; set; }
    public CultureInfo? Locale { get; set; }

    public IEnumerable<VariableModel> Variables { get; set; } = Enumerable.Empty<VariableModel>();

    public bool IsDemo { get; set; }

    public IEnumerable<ErrorModel> Errors { get; set; } = Enumerable.Empty<ErrorModel>();
    public bool HasErrors { get; set; }

    public IEnumerable<ResultDataModel> Result { get; set; } = Enumerable.Empty<ResultDataModel>();
    public bool HasSucceeded { get; set; }
  }
}
