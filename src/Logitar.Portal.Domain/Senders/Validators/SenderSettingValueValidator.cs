using FluentValidation;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Senders.Validators;

internal class SenderSettingValueValidator : AbstractValidator<string>
{
  public SenderSettingValueValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .WithPropertyName(propertyName);
  }
}
