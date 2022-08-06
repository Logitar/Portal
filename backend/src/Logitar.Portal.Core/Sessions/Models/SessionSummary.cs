namespace Logitar.Portal.Core.Sessions.Models
{
  public class SessionSummary
  {
    public SessionUserSummary? User { get; set; }

    public Guid Id { get; set; }
    public DateTime UpdatedAt { get; set; }

    public bool IsPersistent { get; set; }

    public DateTime? SignedOutAt { get; set; }
    public bool IsActive { get; set; }

    public string? IpAddress { get; set; }
  }
}
