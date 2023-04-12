using Logitar.Portal.Core.Actors.Models;

namespace Logitar.Portal.Core
{
  public abstract class AggregateModel
  {
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public ActorModel? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ActorModel? UpdatedBy { get; set; }
    public int Version { get; set; }
  }
}
