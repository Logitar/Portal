using Logitar.Portal.Core.Actors.Models;

namespace Logitar.Portal.Core.Sessions.Models
{
  public class SessionSummary : AggregateSummary
  {
    public SessionUserSummary? User { get; set; }

    public bool IsPersistent { get; set; }

    public DateTime? SignedOutAt { get; set; }
    public ActorModel? SignedOutBy { get; set; }
    public bool IsActive { get; set; }

    public string? IpAddress { get; set; }
  }
}
