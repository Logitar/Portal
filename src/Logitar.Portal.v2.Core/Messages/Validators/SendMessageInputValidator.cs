using FluentValidation;
using Logitar.Portal.v2.Contracts.Messages;

namespace Logitar.Portal.v2.Core.Messages.Validators;

internal class SendMessageInputValidator : AbstractValidator<SendMessageInput>
{
  public SendMessageInputValidator()
  {
    RuleFor(x => x.Realm).NotEmpty();

    RuleFor(x => x.Template).NotEmpty();

    RuleFor(x => x.Recipients).Must(IncludeAtLeastOneToRecipient).WithErrorCode("RecipientsValidator")
      .WithMessage($"'{{PropertyName}}' must include at least one {RecipientType.To} recipient.");
    RuleForEach(x => x.Recipients).SetValidator(new RecipientInputValidator());

    RuleForEach(x => x.Variables).SetValidator(new VariableValidator());
  }

  private static bool IncludeAtLeastOneToRecipient(IEnumerable<RecipientInput>? recipients)
    => recipients == null || recipients.Any(r => r.Type == RecipientType.To);
}
