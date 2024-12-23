using Logitar.Portal.Contracts.Actors;

namespace Logitar.Portal.Contracts.Users;

public abstract record ContactModel
{
  public bool IsVerified { get; set; }
  public ActorModel? VerifiedBy { get; set; }
  public DateTime? VerifiedOn { get; set; }
}
