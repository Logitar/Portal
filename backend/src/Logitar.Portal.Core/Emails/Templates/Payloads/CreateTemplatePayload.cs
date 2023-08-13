namespace Logitar.Portal.Core.Emails.Templates.Payloads
{
  public class CreateTemplatePayload : SaveTemplatePayload
  {
    public string? Realm { get; set; }

    public string Key { get; set; } = null!;
  }
}
