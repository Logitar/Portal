using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Contracts.Sessions;

public class SessionModel : AggregateModel
{
  public bool IsPersistent { get; set; }
  public string? RefreshToken { get; set; }

  public bool IsActive { get; set; }
  public ActorModel? SignedOutBy { get; set; }
  public DateTime? SignedOutOn { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public UserModel User { get; set; }

  public SessionModel() : this(new UserModel())
  {
  }

  public SessionModel(UserModel user)
  {
    User = user;
    CustomAttributes = [];
  }
}
