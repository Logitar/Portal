namespace Logitar.Portal.Contracts.Actors;

public record Actor
{
  public string Id { get; set; } = string.Empty;
  public ActorType Type { get; set; }
  public bool IsDeleted { get; set; }

  public string DisplayName { get; set; } = string.Empty;
  public string? EmailAddress { get; set; }
  public string? PictureUrl { get; set; }
}
