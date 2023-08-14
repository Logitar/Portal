using Logitar.EventSourcing;

namespace Logitar.Portal.Domain;

public record Actor
{
  public string Id { get; set; } = ActorId.DefaultValue;
  public ActorType Type { get; set; } = ActorType.System;
  public bool IsDeleted { get; set; } = false;

  public string DisplayName { get; set; } = "System";
  public string? EmailAddress { get; set; }
  public string? PictureUrl { get; set; }
}
