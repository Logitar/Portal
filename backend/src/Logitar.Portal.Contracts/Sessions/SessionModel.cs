using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Contracts.Sessions;

public class SessionModel : Aggregate
{
  public bool IsPersistent { get; set; }
  public string? RefreshToken { get; set; }

  public bool IsActive { get; set; }
  public Actor? SignedOutBy { get; set; }
  public DateTime? SignedOutOn { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public User User { get; set; }

  public SessionModel() : this(new User())
  {
  }

  public SessionModel(User user)
  {
    User = user;
    CustomAttributes = [];
  }
}
