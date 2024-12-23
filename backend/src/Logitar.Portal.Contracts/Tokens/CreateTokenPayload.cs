﻿using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Contracts.Tokens;

public record CreateTokenPayload
{
  public bool IsConsumable { get; set; }

  public string? Algorithm { get; set; }
  public string? Audience { get; set; }
  public string? Issuer { get; set; }
  public int? LifetimeSeconds { get; set; }
  public string? Secret { get; set; }
  public string? Type { get; set; }

  public string? Subject { get; set; }
  public EmailPayload? Email { get; set; }
  public List<ClaimModel> Claims { get; set; } = [];
}
