using Logitar.Portal.Core.Actors.Models;

namespace Logitar.Portal.Core
{
  public abstract class AggregateSummary
  {
    public Guid Id { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ActorModel? UpdatedBy { get; set; }
  }
}
