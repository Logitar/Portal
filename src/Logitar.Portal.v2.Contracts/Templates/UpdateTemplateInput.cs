namespace Logitar.Portal.v2.Contracts.Templates;

public record UpdateTemplateInput
{
  public string Subject { get; set; } = string.Empty;
  public string ContentType { get; set; } = string.Empty;
  public string? Contents { get; set; }

  public string? DisplayName { get; set; }
  public string? Description { get; set; }
}
