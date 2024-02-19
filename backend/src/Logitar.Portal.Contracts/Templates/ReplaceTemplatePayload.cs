namespace Logitar.Portal.Contracts.Templates;

public record ReplaceTemplatePayload
{
  public string UniqueKey { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string Subject { get; set; }
  public Content Content { get; set; }

  public ReplaceTemplatePayload() : this(string.Empty, string.Empty, new Content())
  {
  }

  public ReplaceTemplatePayload(string uniqueKey, string subject, Content content)
  {
    UniqueKey = uniqueKey;
    Subject = subject;
    Content = content;
  }
}
