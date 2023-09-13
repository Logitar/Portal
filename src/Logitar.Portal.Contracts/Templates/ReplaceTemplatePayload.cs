namespace Logitar.Portal.Contracts.Templates;

public record ReplaceTemplatePayload
{
  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string Subject { get; set; } = string.Empty;
  public string ContentType { get; set; } = string.Empty;
  public string Contents { get; set; } = string.Empty;
}
