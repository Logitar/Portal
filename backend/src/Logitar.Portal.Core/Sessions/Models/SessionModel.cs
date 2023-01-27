using Logitar.Portal.Core.Models;
using Logitar.Portal.Core.Users.Models;

namespace Logitar.Portal.Core.Sessions.Models
{
  public class SessionModel : AggregateModel
  {
    public UserModel? User { get; set; }

    public string? KeyHash { get; set; }
    public bool IsPersistent { get; set; }

    public string? SignedOutBy { get; set; }
    public DateTime? SignedOutOn { get; set; }
    public bool IsActive { get; set; }

    public string? IpAddress { get; set; }
    public string? AdditionalInformation { get; set; }

    public string? RenewToken { get; set; }
  }
}
