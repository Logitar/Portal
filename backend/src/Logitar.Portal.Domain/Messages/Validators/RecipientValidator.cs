using FluentValidation;

namespace Logitar.Portal.Domain.Messages.Validators;

internal class RecipientValidator : AbstractValidator<Recipient>
{
  public RecipientValidator()
  {
    RuleFor(x => x.Type).IsInEnum();

    RuleFor(x => x).Must(x => x.Address != null || x.PhoneNumber != null)
      .WithErrorCode(nameof(RecipientValidator))
      .WithMessage(x => $"At least one of the following must be specified: {nameof(x.Address)}, {nameof(x.PhoneNumber)}.");

    When(x => x.Address != null, () => RuleFor(x => x.Address!).EmailAddressInput());
    When(x => x.DisplayName != null, () => RuleFor(x => x.DisplayName).NotEmpty());

    When(x => x.PhoneNumber != null, () => RuleFor(x => x.PhoneNumber!).PhoneNumber());

    When(x => x.User != null, () => RuleFor(x => x.UserId).NotNull());
  }
}
