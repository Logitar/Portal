namespace Logitar.Portal.Core.ApiKeys.Models
{
  public class ApiKeyModel : AggregateModel
  {
    public string? XApiKey { get; set; }

    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public DateTime? ExpiresAt { get; set; }
    public bool IsExpired => ExpiresAt <= DateTime.UtcNow;
  }
}
