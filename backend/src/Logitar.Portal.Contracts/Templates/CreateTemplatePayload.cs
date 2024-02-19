namespace Logitar.Portal.Contracts.Templates;

public record CreateTemplatePayload
{
  public string UniqueKey { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string Subject { get; set; }
  public Content Content { get; set; }

  public CreateTemplatePayload() : this(string.Empty, string.Empty, new Content())
  {
  }

  public CreateTemplatePayload(string uniqueKey, string subject, Content content)
  {
    UniqueKey = uniqueKey;
    Subject = subject;
    Content = content;
  }
}
