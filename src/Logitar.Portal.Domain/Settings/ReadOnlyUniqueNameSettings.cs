using FluentValidation;
using Logitar.Portal.Contracts.Settings;
using Logitar.Portal.Domain.Settings.Validators;

namespace Logitar.Portal.Domain.Settings;

public record ReadOnlyUniqueNameSettings : IUniqueNameSettings
{
  public ReadOnlyUniqueNameSettings(string? allowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+")
  {
    AllowedCharacters = allowedCharacters?.CleanTrim();

    new ReadOnlyUniqueNameSettingsValidator().ValidateAndThrow(this);
  }

  public string? AllowedCharacters { get; }
}
