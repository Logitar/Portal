using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Contracts.Messages;

public record Message : Aggregate
{
  public Guid Id { get; set; }

  public bool IsDemo { get; set; }

  public string Subject { get; set; } = string.Empty;
  public string Body { get; set; } = string.Empty;

  public IEnumerable<Recipient> Recipients { get; set; } = Enumerable.Empty<Recipient>();
  public int RecipientCount { get; set; }

  public Guid RealmId { get; set; }
  public string RealmUniqueName { get; set; } = string.Empty;
  public string? RealmDisplayName { get; set; }

  public Guid SenderId { get; set; }
  public bool SenderIsDefault { get; set; }
  public ProviderType SenderProvider { get; set; }
  public string SenderAddress { get; set; } = string.Empty;
  public string? SenderDisplayName { get; set; }

  public Guid TemplateId { get; set; }
  public string TemplateKey { get; set; } = string.Empty;
  public string? TemplateDisplayName { get; set; }
  public string TemplateContentType { get; set; } = string.Empty;

  public bool IgnoreUserLocale { get; set; }
  public string? Locale { get; set; }

  public IEnumerable<Variable> Variables { get; set; } = Enumerable.Empty<Variable>();

  public IEnumerable<Error> Errors { get; set; } = Enumerable.Empty<Error>();
  public bool HasErrors => Errors.Any();

  public IEnumerable<ResultData> Result { get; set; } = Enumerable.Empty<ResultData>();
  public bool Succeeded => !HasErrors && Result != null;
}
