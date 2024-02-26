using FluentValidation;
using Logitar.Portal.Domain.Settings.Validators;
using Logitar.Security.Cryptography;

namespace Logitar.Portal.Domain.Settings;

public record JwtSecretUnit
{
  public const int MaximumLength = 512 / 8;
  public const int MinimumLength = 256 / 8;

  public string Value { get; }

  public JwtSecretUnit(string value, string? propertyName = null)
  {
    Value = value.Trim();
    new JwtSecretValidator(propertyName).ValidateAndThrow(Value);
  }

  public static JwtSecretUnit CreateOrGenerate(string? value, string? propertyName = null)
  {
    return string.IsNullOrWhiteSpace(value) ? Generate() : new(value, propertyName);
  }

  public static JwtSecretUnit Generate() => new(RandomStringGenerator.GetString());
  public static JwtSecretUnit Generate(int length) => new(RandomStringGenerator.GetString(length));
}
