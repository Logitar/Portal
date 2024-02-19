namespace Logitar.Portal.Contracts.Templates;

public record CreateTemplatePayload
{
  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string Subject { get; set; }
  public Content Content { get; set; }

  public CreateTemplatePayload() : this(string.Empty, string.Empty, new Content())
  {
  }

  public CreateTemplatePayload(string uniqueName, string subject, Content content)
  {
    UniqueName = uniqueName;
    Subject = subject;
    Content = content;
  }
}
