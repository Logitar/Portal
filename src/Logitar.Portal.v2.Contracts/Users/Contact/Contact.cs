using Logitar.Portal.v2.Contracts.Actors;

namespace Logitar.Portal.v2.Contracts.Users.Contact;

public abstract record Contact
{
  public Actor? VerifiedBy { get; set; }
  public DateTime? VerifiedOn { get; set; }
  public bool IsVerified { get; set; }
}
