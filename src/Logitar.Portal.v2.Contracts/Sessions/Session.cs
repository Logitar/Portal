using Logitar.Portal.v2.Contracts.Actors;
using Logitar.Portal.v2.Contracts.Users;

namespace Logitar.Portal.v2.Contracts.Sessions;

public record Session : Aggregate
{
  public Guid Id { get; set; }

  public bool IsPersistent { get; set; }

  public Actor? SignedOutBy { get; set; }
  public DateTime? SignedOutOn { get; set; }
  public bool IsActive { get; set; }

  public string? RefreshToken { get; set; }

  public IEnumerable<CustomAttribute> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttribute>();

  public User User { get; set; } = new();

  // TODO(fpion): IpAddress as column & sortable property?
  // TODO(fpion): AdditionalInformation as column and/or property?
}
