using FluentValidation;
using Logitar.Portal.Contracts.Settings;

namespace Logitar.Portal.Domain.Settings;

public record ReadOnlyPasswordSettings : IPasswordSettings
{
  public ReadOnlyPasswordSettings(int requiredLength = 8, int requiredUniqueChars = 8, bool requireNonAlphanumeric = true,
    bool requireLowercase = true, bool requireUppercase = true, bool requireDigit = true, string strategy = "PBKDF2")
  {
    RequiredLength = requiredLength;
    RequiredUniqueChars = requiredUniqueChars;
    RequireNonAlphanumeric = requireNonAlphanumeric;
    RequireLowercase = requireLowercase;
    RequireUppercase = requireUppercase;
    RequireDigit = requireDigit;

    Strategy = strategy;

    new ReadOnlyPasswordSettingsValidator().ValidateAndThrow(this);
  }

  public int RequiredLength { get; }
  public int RequiredUniqueChars { get; }
  public bool RequireNonAlphanumeric { get; }
  public bool RequireLowercase { get; }
  public bool RequireUppercase { get; }
  public bool RequireDigit { get; }

  public string Strategy { get; }
}

internal class ReadOnlyPasswordSettingsValidator : AbstractValidator<ReadOnlyPasswordSettings>
{
  public ReadOnlyPasswordSettingsValidator()
  {
    RuleFor(x => x.RequiredLength).GreaterThan(0);

    RuleFor(x => x.RequiredUniqueChars).GreaterThan(0)
      .LessThanOrEqualTo(x => x.RequiredLength);

    RuleFor(x => x.Strategy).NotEmpty()
      .MaximumLength(byte.MaxValue);
  }
}
