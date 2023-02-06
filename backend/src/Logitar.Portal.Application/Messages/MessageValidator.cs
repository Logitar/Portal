using FluentValidation;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.Application.Messages
{
  internal class MessageValidator : AbstractValidator<Message>
  {
    public MessageValidator(IValidator<Recipient> recipientValidator)
    {
      RuleFor(x => x.Subject).NotEmpty()
        .MaximumLength(255);

      RuleFor(x => x.Body).NotEmpty();

      RuleFor(x => x.Recipients).Must(x => x.Any(y => y.Type == RecipientType.To))
        .WithErrorCode("RecipientsValidator")
        .WithMessage($"'{{PropertyName}}' must contain at least one {RecipientType.To} recipient.");
      RuleForEach(x => x.Recipients).SetValidator(recipientValidator);

      RuleFor(x => x.SenderAddress).NotEmpty()
        .MaximumLength(255)
        .EmailAddress();

      RuleFor(x => x.SenderDisplayName).NullOrNotEmpty()
        .MaximumLength(255);

      RuleFor(x => x.SenderProvider).IsInEnum();

      RuleFor(x => x.TemplateKey).NotEmpty()
        .MaximumLength(255)
        .Identifier();

      RuleFor(x => x.TemplateDisplayName).NullOrNotEmpty()
        .MaximumLength(255);

      RuleFor(x => x.TemplateContentType).NotEmpty()
        .ContentType();

      RuleFor(x => x.RealmAlias).NotEmpty()
        .MaximumLength(255)
        .Alias();

      RuleFor(x => x.RealmDisplayName).NullOrNotEmpty()
        .MaximumLength(255);

      RuleFor(x => x.Locale).Locale();

      When(x => x.Variables != null, () => RuleForEach(x => x.Variables!.Keys).NotEmpty()
        .MaximumLength(255)
        .Identifier());

      RuleFor(x => x).Must(x => x.Result == null || !x.HasErrors)
        .WithErrorCode("MessageStatusValidator")
        .WithMessage("The message can only have errors or a success result.");
    }
  }
}
