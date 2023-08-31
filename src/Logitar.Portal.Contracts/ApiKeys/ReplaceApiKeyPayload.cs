﻿namespace Logitar.Portal.Contracts.ApiKeys;

public record ReplaceApiKeyPayload
{
  public string Title { get; set; } = string.Empty;
  public string? Description { get; set; }
  public DateTime? ExpiresOn { get; set; }

  public IEnumerable<CustomAttribute> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttribute>();

  public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
}
