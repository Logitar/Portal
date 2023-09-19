using FluentValidation;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Senders.Validators;

internal class SenderSettingKeyValidator : AbstractValidator<string>
{
  public SenderSettingKeyValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier()
      .WithPropertyName(propertyName);
  }
}
