namespace Logitar.Portal.Core.Emails.Templates.Models
{
  public class TemplateSummary : AggregateSummary
  {
    public string Key { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string ContentType { get; set; } = null!;
  }
}
