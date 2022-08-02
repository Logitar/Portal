namespace Logitar.Portal.Web.Models.Api.Realm
{
  public class RealmSummary
  {
    public Guid Id { get; set; }
    public DateTime UpdatedAt { get; set; }

    public string Alias { get; set; } = null!;
    public string Name { get; set; } = null!;
  }
}
