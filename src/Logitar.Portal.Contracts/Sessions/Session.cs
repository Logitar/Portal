using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Contracts.Sessions;

public record Session : Aggregate
{
  public string? RefreshToken { get; set; }
  public bool IsPersistent { get; set; }

  public bool IsActive { get; set; }
  public Actor? SignedOutBy { get; set; }
  public DateTime? SignedOutOn { get; set; }

  public IEnumerable<CustomAttribute> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttribute>();

  public User User { get; set; } = new();
}
