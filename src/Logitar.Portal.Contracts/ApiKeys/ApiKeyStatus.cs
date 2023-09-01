﻿namespace Logitar.Portal.Contracts.ApiKeys;

public record ApiKeyStatus
{
  public bool IsExpired { get; set; }
  public DateTime? Moment { get; set; }
}
