﻿using FluentValidation;
using Logitar.Portal.Domain.Validators;
using Logitar.Security.Cryptography;

namespace Logitar.Portal.Domain.Settings;

public record JwtSecret
{
  public JwtSecret(string value)
  {
    Value = value.Trim();

    new JwtSecretValidator("Secret").ValidateAndThrow(this);
  }

  public string Value { get; }

  public static JwtSecret Generate(int length = 256 / 8) => new(RandomStringGenerator.GetString(length));
}

internal class JwtSecretValidator : AbstractValidator<JwtSecret>
{
  public JwtSecretValidator(string? propertyName = null)
  {
    RuleFor(x => x.Value).NotEmpty()
      .MinimumLength(256 / 8)
      .MaximumLength(512 / 8)
      .WithPropertyName(propertyName);
  }
}
