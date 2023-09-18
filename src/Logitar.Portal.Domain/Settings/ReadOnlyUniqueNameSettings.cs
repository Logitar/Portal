using FluentValidation;
using Logitar.Portal.Contracts.Settings;

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

internal class ReadOnlyUniqueNameSettingsValidator : AbstractValidator<ReadOnlyUniqueNameSettings>
{
  public ReadOnlyUniqueNameSettingsValidator()
  {
    When(x => x.AllowedCharacters != null, () => RuleFor(x => x.AllowedCharacters).NotEmpty()
      .MaximumLength(byte.MaxValue));
  }
}
