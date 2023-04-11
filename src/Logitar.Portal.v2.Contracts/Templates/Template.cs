﻿using Logitar.Portal.v2.Contracts.Realms;

namespace Logitar.Portal.v2.Contracts.Templates;

public record Template : Aggregate
{
  public string Key { get; set; } = string.Empty;

  public string Subject { get; set; } = string.Empty;
  public string ContentType { get; set; } = string.Empty;
  public string Contents { get; set; } = string.Empty;

  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public Realm Realm { get; set; } = new();
}