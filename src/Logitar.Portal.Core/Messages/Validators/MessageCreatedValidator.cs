using FluentValidation;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Core.Messages.Events;

namespace Logitar.Portal.Core.Messages.Validators;

internal class MessageCreatedValidator : AbstractValidator<MessageCreated>
{
  public MessageCreatedValidator()
  {
    RuleFor(x => x.Subject).NotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.Body).NotEmpty();

    RuleFor(x => x.Recipients).Must(IncludeAtLeastOneToRecipient).WithErrorCode("RecipientsValidator")
      .WithMessage($"'{{PropertyName}}' must include at least one {RecipientType.To} recipient.");

    RuleFor(x => x.Locale).Locale();

    RuleForEach(x => x.Variables.Keys).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier();
    RuleForEach(x => x.Variables.Values).NotEmpty();
  }

  private static bool IncludeAtLeastOneToRecipient(IEnumerable<Recipient>? recipients)
    => recipients == null || recipients.Any(r => r.Type == RecipientType.To);
}
