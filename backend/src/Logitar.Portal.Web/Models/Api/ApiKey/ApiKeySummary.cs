namespace Logitar.Portal.Web.Models.Api.ApiKey
{
  public class ApiKeySummary
  {
    public Guid Id { get; set; }
    public DateTime UpdatedAt { get; set; }

    public string Name { get; set; } = null!;
    public DateTime? ExpiresAt { get; set; }
  }
}
