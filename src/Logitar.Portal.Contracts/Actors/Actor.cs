namespace Logitar.Portal.Contracts.Actors;

public record Actor
{
  public string Id { get; set; } = "SYSTEM";
  public ActorType Type { get; set; } = ActorType.System;
  public bool IsDeleted { get; set; } = false;

  public string DisplayName { get; set; } = "System";
  public string? EmailAddress { get; set; }
  public string? PictureUrl { get; set; }
}
