using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Application.Messages.Validators;

internal class SendMessageValidator : AbstractValidator<SendMessagePayload>
{
  public SendMessageValidator()
  {
    RuleFor(x => x.Template).NotEmpty();

    RuleFor(x => x.Recipients).Must(r => r.Any(x => x.Type == RecipientType.To))
      .WithErrorCode("RecipientsValidator")
      .WithMessage($"'{{PropertyName}}' must contain at least one {nameof(RecipientType.To)} recipient.");
    RuleForEach(x => x.Recipients).SetValidator(new RecipientValidator());

    When(x => !string.IsNullOrWhiteSpace(x.Locale), () => RuleFor(x => x.Locale!).SetValidator(new LocaleValidator()));

    RuleForEach(x => x.Variables).SetValidator(new VariableValidator());
  }
}
