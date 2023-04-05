namespace Logitar.Portal.Core.ApiKeys.Models
{
  public class ApiKeySummary : AggregateSummary
  {
    public string Name { get; set; } = null!;
    public DateTime? ExpiresAt { get; set; }
  }
}
