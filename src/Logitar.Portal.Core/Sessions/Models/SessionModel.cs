using Logitar.Portal.Core.Actors.Models;
using Logitar.Portal.Core.Users.Models;

namespace Logitar.Portal.Core.Sessions.Models
{
  public class SessionModel : AggregateModel
  {
    public UserModel? User { get; set; }

    public string? RenewToken { get; set; }
    public bool IsPersistent { get; set; }

    public DateTime? SignedOutAt { get; set; }
    public ActorModel? SignedOutBy { get; set; }
    public bool IsActive { get; set; }

    public string? IpAddress { get; set; }
    public string? AdditionalInformation { get; set; }
  }
}
