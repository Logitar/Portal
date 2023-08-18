using Logitar.Portal.Contracts.Actors;

namespace Logitar.Portal.Contracts.Users;

public abstract record Contact
{
  public Actor? VerifiedBy { get; set; }
  public DateTime? VerifiedOn { get; set; }
  public bool IsVerified { get; set; }
}
