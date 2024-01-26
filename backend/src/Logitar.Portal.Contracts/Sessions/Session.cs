using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Contracts.Sessions;

public class Session : Aggregate
{
  public string? RefreshToken { get; set; }
  public bool IsPersistent { get; set; }

  public bool IsActive { get; set; }
  public Actor? SignedOutBy { get; set; }
  public DateTime? SignedOutOn { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public User User { get; set; }

  public Session() : this(new User())
  {
  }

  public Session(User user)
  {
    CustomAttributes = [];
    User = user;
  }
}
