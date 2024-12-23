namespace Logitar.Portal.Contracts.Templates;

public record UpdateTemplatePayload
{
  public string? UniqueKey { get; set; }
  public ChangeModel<string>? DisplayName { get; set; }
  public ChangeModel<string>? Description { get; set; }

  public string? Subject { get; set; }
  public Content? Content { get; set; }
}
