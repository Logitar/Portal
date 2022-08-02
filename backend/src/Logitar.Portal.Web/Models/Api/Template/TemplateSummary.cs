namespace Logitar.Portal.Web.Models.Api.Template
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
