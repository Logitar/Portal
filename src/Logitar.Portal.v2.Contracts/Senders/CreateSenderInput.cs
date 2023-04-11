﻿namespace Logitar.Portal.v2.Contracts.Senders;

public record CreateSenderInput
{
  public string Realm { get; set; } = string.Empty;

  public ProviderType Provider { get; set; }

  public string EmailAddress { get; set; } = string.Empty;
  public string? DisplayName { get; set; }

  public IEnumerable<Setting>? Settings { get; set; }
}