namespace Logitar.Portal.Core.ApiKeys.Models
{
  public class ApiKeySummary
  {
    public Guid Id { get; set; }
    public DateTime UpdatedAt { get; set; }

    public string Name { get; set; } = null!;
    public DateTime? ExpiresAt { get; set; }
  }
}
