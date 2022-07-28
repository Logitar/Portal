namespace Portal.Core.Templates.Payloads
{
  public abstract class SaveTemplatePayload
  {
    public string Contents { get; set; } = null!;

    public string? DisplayName { get; set; }
    public string? Description { get; set; }
  }
}
