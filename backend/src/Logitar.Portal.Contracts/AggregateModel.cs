using System;

namespace Logitar.Portal.Contracts
{
  public abstract class AggregateModel
  {
    public string Id { get; set; } = string.Empty;
    public long Version { get; set; }

    //public ActorModel? CreatedBy { get; set; } // TODO(fpion): implement
    public DateTime CreatedOn { get; set; }

    //public ActorModel? UpdatedBy { get; set; } // TODO(fpion): implement
    public DateTime? UpdatedOn { get; set; }
  }
}
