﻿using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Sessions;

public record SearchSessionsPayload : SearchPayload
{
  public Guid? UserId { get; set; }
  public bool? IsActive { get; set; }
  public bool? IsPersistent { get; set; }

  public new List<SessionSortOption> Sort { get; set; } = [];
}
