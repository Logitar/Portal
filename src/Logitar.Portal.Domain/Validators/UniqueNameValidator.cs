using FluentValidation;
using Logitar.Portal.Contracts.Settings;

namespace Logitar.Portal.Domain.Validators;

internal class UniqueNameValidator : AbstractValidator<string>
{
  public UniqueNameValidator(IUniqueNameSettings uniqueNameSettings, string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .AllowedCharacters(uniqueNameSettings.AllowedCharacters)
      .WithPropertyName(propertyName);
  }
}
