using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users.Models;
using System;

namespace Logitar.Portal.Contracts.Sessions.Models
{
  public class SessionModel : AggregateModel
  {
    public UserModel? User { get; set; }

    public string? RenewToken { get; set; }
    public bool IsPersistent { get; set; }

    public ActorModel? SignedOutBy { get; set; }
    public DateTime? SignedOutOn { get; set; }
    public bool IsActive { get; set; }

    public string? IpAddress { get; set; }
    public string? AdditionalInformation { get; set; }
  }
}
