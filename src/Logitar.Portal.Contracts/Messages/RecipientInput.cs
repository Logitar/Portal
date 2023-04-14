﻿namespace Logitar.Portal.Contracts.Messages;

public record RecipientInput
{
  public RecipientType Type { get; set; }
  public string? User { get; set; }

  public string? Address { get; set; }
  public string? DisplayName { get; set; }
}