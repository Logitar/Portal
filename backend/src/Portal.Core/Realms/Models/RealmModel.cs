namespace Portal.Core.Realms.Models
{
  public class RealmModel : AggregateModel
  {
    public string Alias { get; private set; } = null!;

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
  }
}
