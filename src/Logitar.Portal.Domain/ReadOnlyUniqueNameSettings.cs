﻿using FluentValidation;
using Logitar.Identity.Domain.Settings;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain;

public record ReadOnlyUniqueNameSettings : IUniqueNameSettings
{
  public ReadOnlyUniqueNameSettings(string? allowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+")
  {
    AllowedCharacters = allowedCharacters?.CleanTrim();

    new ReadOnlyUniqueNameSettingsValidator().ValidateAndThrow(this);
  }


  public string? AllowedCharacters { get; }
}