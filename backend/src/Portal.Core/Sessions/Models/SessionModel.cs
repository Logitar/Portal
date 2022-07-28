using Portal.Core.Users.Models;

namespace Portal.Core.Sessions.Models
{
  public class SessionModel : AggregateModel
  {
    public UserModel? User { get; set; }

    public string? RenewToken { get; set; }
    public bool IsPersistent => RenewToken != null;

    public DateTime? SignedOutAt { get; set; }
    public Guid? SignedOutById { get; set; }
    public bool IsActive => !SignedOutAt.HasValue && !SignedOutById.HasValue;

    public string? IpAddress { get; set; }
    public string? AdditionalInformation { get; set; }
  }
}
