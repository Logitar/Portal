﻿using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Realms;

namespace Logitar.Portal.v2.Core.Realms;

public record ReadOnlyUsernameSettings
{
  public ReadOnlyUsernameSettings()
  {
  }
  public ReadOnlyUsernameSettings(UsernameSettings usernameSettings)
  {
    AllowedCharacters = usernameSettings.AllowedCharacters?.CleanTrim();
  }

  public string? AllowedCharacters { get; init; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
}