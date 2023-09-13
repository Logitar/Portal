namespace Logitar.Portal.Contracts.Templates;

public record UpdateTemplatePayload
{
  public string? UniqueName { get; set; }
  public Modification<string>? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }

  public string? Subject { get; set; }
  public string? ContentType { get; set; }
  public string? Contents { get; set; }
}
