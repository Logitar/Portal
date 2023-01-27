namespace Logitar.Portal.Core.Actors.Models
{
  public class ActorModel
  {
    public string Id { get; set; } = null!;

    public ActorType Type { get; set; }
    public bool IsDeleted { get; set; }

    public string DisplayName { get; set; } = null!;
    public string? Email { get; set; }
    public string? Picture { get; set; }
  }
}
