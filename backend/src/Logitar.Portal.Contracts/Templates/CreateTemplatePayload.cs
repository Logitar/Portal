namespace Logitar.Portal.Contracts.Templates;

public record CreateTemplatePayload
{
  public Guid? Id { get; set; }

  public string UniqueKey { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string Subject { get; set; }
  public ContentModel Content { get; set; }

  public CreateTemplatePayload() : this(string.Empty, string.Empty, new ContentModel())
  {
  }

  public CreateTemplatePayload(string uniqueKey, string subject, ContentModel content)
  {
    UniqueKey = uniqueKey;
    Subject = subject;
    Content = content;
  }
}
