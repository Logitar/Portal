using FluentValidation;
using Logitar.Portal.Domain.Settings.Validators;
using Logitar.Security.Cryptography;

namespace Logitar.Portal.Domain.Settings;

public record JwtSecret
{
  public const int MaximumLength = 512 / 8;
  public const int MinimumLength = 256 / 8;

  public string Value { get; }

  public JwtSecret(string value, string? propertyName = null)
  {
    Value = value.Trim();
    new JwtSecretValidator(propertyName).ValidateAndThrow(Value);
  }

  public static JwtSecret CreateOrGenerate(string? value, string? propertyName = null)
  {
    return string.IsNullOrWhiteSpace(value) ? Generate() : new(value, propertyName);
  }

  public static JwtSecret Generate() => new(RandomStringGenerator.GetString());
  public static JwtSecret Generate(int length) => new(RandomStringGenerator.GetString(length));
}
