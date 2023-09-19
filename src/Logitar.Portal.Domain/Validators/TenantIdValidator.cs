using FluentValidation;

namespace Logitar.Portal.Domain.Validators;

internal class TenantIdValidator : AbstractValidator<string>
{
  private const string UriSafeCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._~";

  public TenantIdValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .AllowedCharacters(UriSafeCharacters)
      .WithPropertyName(propertyName);
  }
}
