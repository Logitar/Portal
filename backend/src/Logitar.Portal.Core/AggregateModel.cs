namespace Logitar.Portal.Core
{
  public abstract class AggregateModel
  {
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int Version { get; set; }
  }
}
