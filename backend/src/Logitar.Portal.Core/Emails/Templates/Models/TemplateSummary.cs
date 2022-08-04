namespace Logitar.Portal.Core.Emails.Templates.Models
{
  public class TemplateSummary
  {
    public Guid Id { get; set; }
    public DateTime UpdatedAt { get; set; }

    public string Key { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string ContentType { get; set; } = null!;
  }
}
