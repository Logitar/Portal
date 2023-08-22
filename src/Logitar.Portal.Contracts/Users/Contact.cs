using Logitar.Portal.Contracts.Actors;

namespace Logitar.Portal.Contracts.Users;

public abstract record Contact
{
  public bool IsVerified { get; set; }
  public Actor? VerifiedBy { get; set; }
  public DateTime? VerifiedOn { get; set; }
}
