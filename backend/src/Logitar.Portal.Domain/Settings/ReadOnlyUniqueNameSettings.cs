﻿using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Domain.Settings.Validators;

namespace Logitar.Portal.Domain.Settings;

public record ReadOnlyUniqueNameSettings : IUniqueNameSettings
{
  public string? AllowedCharacters { get; }

  public ReadOnlyUniqueNameSettings() : this(new UniqueNameSettings())
  {
  }

  public ReadOnlyUniqueNameSettings(IUniqueNameSettings uniqueName) : this(uniqueName.AllowedCharacters)
  {
  }

  public ReadOnlyUniqueNameSettings(string? allowedCharacters)
  {
    AllowedCharacters = allowedCharacters;

    new UniqueNameSettingsValidator().ValidateAndThrow(this);
  }
}
