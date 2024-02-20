using FluentValidation;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Application.Messages.Validators;

internal class RecipientValidator : AbstractValidator<RecipientPayload>
{
  public RecipientValidator()
  {
    RuleFor(x => x.Type).IsInEnum();

    When(x => !string.IsNullOrWhiteSpace(x.Address), () => RuleFor(x => x.Address).EmailAddress());

    RuleFor(x => x).Must(x => x.UserId.HasValue || !string.IsNullOrWhiteSpace(x.Address))
      .WithErrorCode(nameof(RecipientValidator))
      .WithMessage($"At least one of the following must be specified: {nameof(RecipientPayload.UserId)}, {nameof(RecipientPayload.Address)}.");
  }
}
