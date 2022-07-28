namespace Portal.Core.Templates.Payloads
{
  public class CreateTemplatePayload : SaveTemplatePayload
  {
    public string? Realm { get; set; }

    public string Key { get; set; } = null!;
  }
}
