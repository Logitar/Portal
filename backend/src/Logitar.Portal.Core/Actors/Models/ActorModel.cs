namespace Logitar.Portal.Core.Actors.Models
{
  public class ActorModel
  {
    public Guid Id { get; set; }

    public ActorType Type { get; set; }
    public string Name { get; set; } = null!;
    public bool IsDeleted { get; set; }

    public string? Email { get; set; }
    public string? Picture { get; set; }
  }
}
