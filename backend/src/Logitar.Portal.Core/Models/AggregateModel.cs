using Logitar.Portal.Core.Actors.Models;

namespace Logitar.Portal.Core.Models
{
  public abstract class AggregateModel
  {
    public string Id { get; set; } = null!;
    public long Version { get; set; }

    public ActorModel? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }

    public ActorModel? UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }

    public virtual AggregateId GetAggregateId() => new(Id);
  }
}
