using FluentValidation;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Senders;

namespace Logitar.Portal.Application.Messages.Validators;

internal class RecipientValidator : AbstractValidator<RecipientPayload>
{
  public RecipientValidator(SenderType senderType)
  {
    RuleFor(x => x.Type).IsInEnum();

    When(x => !string.IsNullOrWhiteSpace(x.Address), () => RuleFor(x => x.Address).EmailAddress());

    switch (senderType)
    {
      case SenderType.Email:
        RuleFor(x => x.PhoneNumber).Empty();
        RuleFor(x => x).Must(x => !string.IsNullOrWhiteSpace(x.Address) || x.UserId.HasValue)
          .WithErrorCode(nameof(RecipientValidator))
          .WithMessage($"At least one of the following must be specified: {nameof(RecipientPayload.Address)}, {nameof(RecipientPayload.UserId)}.");
        break;
      case SenderType.Sms:
        RuleFor(x => x.Address).Empty();
        RuleFor(x => x).Must(x => !string.IsNullOrWhiteSpace(x.PhoneNumber) || x.UserId.HasValue)
          .WithErrorCode(nameof(RecipientValidator))
          .WithMessage($"At least one of the following must be specified: {nameof(RecipientPayload.PhoneNumber)}, {nameof(RecipientPayload.UserId)}.");
        break;
      default:
        throw new SenderTypeNotSupportedException(senderType);
    }
  }
}
