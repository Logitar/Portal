using FluentValidation;
using Logitar.Portal.Domain.Realms.Validators;
using Logitar.Security.Cryptography;

namespace Logitar.Portal.Domain.Realms;

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

  public static JwtSecretUnit Generate() => Generate(MinimumLength);
  public static JwtSecretUnit Generate(int length) => new(RandomStringGenerator.GetString(length));
}
