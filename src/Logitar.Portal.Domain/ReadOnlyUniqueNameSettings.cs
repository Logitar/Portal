using FluentValidation;
using Logitar.Portal.Domain.Realms.Validators;

namespace Logitar.Portal.Domain;

public record class ReadOnlyUniqueNameSettings
{
  public ReadOnlyUniqueNameSettings(string? allowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+")
  {
    AllowedCharacters = allowedCharacters?.CleanTrim();

    new ReadOnlyUniqueNameSettingsValidator().ValidateAndThrow(this);
  }


  public string? AllowedCharacters { get; }
}
