namespace Logitar.Portal.Contracts.Actors;

public record Actor
{
  public Guid Id { get; set; }
  public ActorType Type { get; set; }
  public bool IsDeleted { get; set; }

  public string DisplayName { get; set; } = "System";
  public string? EmailAddress { get; set; }
  public string? PictureUrl { get; set; }
}
