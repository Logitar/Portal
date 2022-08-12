namespace Logitar.Portal.Core.Actors.Models
{
  public class ActorModel
  {
    public Guid Id { get; private set; }

    public ActorType Type { get; private set; }
    public string Name { get; private set; } = null!;
    public bool IsDeleted { get; private set; }

    public string? Email { get; private set; }
    public string? Picture { get; private set; }
  }
}
