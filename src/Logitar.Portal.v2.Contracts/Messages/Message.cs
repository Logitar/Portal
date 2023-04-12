using Logitar.Portal.v2.Contracts.Errors;
using Logitar.Portal.v2.Contracts.Senders;

namespace Logitar.Portal.v2.Contracts.Messages;

public record Message : Aggregate
{
  public Guid Id { get; set; }

  public bool IsDemo { get; set; }

  public string Subject { get; set; } = string.Empty;
  public string Body { get; set; } = string.Empty;

  public IEnumerable<Recipient> Recipients { get; set; } = Enumerable.Empty<Recipient>();

  public Guid RealmId { get; set; } // TODO(fpion): not null
  public string RealmAlias { get; set; } = string.Empty; // TODO(fpion): not null
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

  public IEnumerable<Variable> Variables { get; set; } = Enumerable.Empty<Variable>(); // TODO(fpion): Value is now not null

  public IEnumerable<Error> Errors { get; set; } = Enumerable.Empty<Error>(); // TODO(fpion): Data.Value is now not null
  public bool HasErrors => Errors.Any();

  public IEnumerable<ResultData> Result { get; set; } = Enumerable.Empty<ResultData>(); // TODO(fpion): Value is now not null
  public bool Succeeded => !HasErrors && Result != null;
}
