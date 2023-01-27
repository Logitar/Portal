namespace Logitar.Portal.Core.Realms.Models
{
  public class RealmSummary : AggregateSummary
  {
    public string Alias { get; set; } = null!;
    public string Name { get; set; } = null!;
  }
}
