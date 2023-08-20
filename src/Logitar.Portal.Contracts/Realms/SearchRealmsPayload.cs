﻿namespace Logitar.Portal.Contracts.Realms;

public record SearchRealmsPayload : SearchPayload
{
  public TextSearch Id { get; set; } = new();

  public new IEnumerable<RealmSortOption> Sort { get; set; } = Enumerable.Empty<RealmSortOption>();
}
