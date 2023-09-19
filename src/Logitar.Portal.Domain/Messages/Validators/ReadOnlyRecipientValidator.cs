using FluentValidation;

namespace Logitar.Portal.Domain.Messages.Validators;

internal class ReadOnlyRecipientValidator : AbstractValidator<ReadOnlyRecipient>
{
  public ReadOnlyRecipientValidator()
  {
    RuleFor(x => x.Type).IsInEnum();

    RuleFor(x => x.Address).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .EmailAddress();

    When(x => x.DisplayName != null, () => RuleFor(x => x.DisplayName).NotEmpty()
      .MaximumLength(byte.MaxValue));
  }
}
