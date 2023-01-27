namespace Logitar.Portal.Core.Actors.Models
{
  public record ActorModel
  {
    public string Id { get; init; } = null!;

    public ActorType Type { get; init; }
    public bool IsDeleted { get; init; }

    public string DisplayName { get; init; } = null!;
    public string? Email { get; init; }
    public string? Picture { get; init; }
  }
}
