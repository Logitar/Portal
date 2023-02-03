using Logitar.Portal.Contracts.Actors;

namespace Logitar.Portal.Contracts
{
  public abstract record AggregateModel
  {
    public string Id { get; set; } = string.Empty;
    public long Version { get; set; }

    public ActorModel? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }

    public ActorModel? UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
  }
}
