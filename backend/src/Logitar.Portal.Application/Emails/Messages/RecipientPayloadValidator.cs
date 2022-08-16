using FluentValidation;
using Logitar.Portal.Core.Emails.Messages;
using Logitar.Portal.Core.Emails.Messages.Payloads;

namespace Logitar.Portal.Application.Emails.Messages
{
  internal class RecipientPayloadValidator : AbstractValidator<RecipientPayload>
  {
    public RecipientPayloadValidator()
    {
      When(x => x.Type == RecipientType.To, () =>
      {
        RuleFor(x => x.User).NotEmpty();
        RuleFor(x => x.Address).Null();
        RuleFor(x => x.DisplayName).Null();
      });

      When(x => x.Type != RecipientType.To, () =>
      {
        When(x => x.User == null, () =>
        {
          RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(256)
            .EmailAddress();

          RuleFor(x => x.DisplayName)
            .MaximumLength(256);
        });

        When(x => x.User != null, () =>
        {
          RuleFor(x => x.Address).Null();
          RuleFor(x => x.DisplayName).Null();
        });
      });
    }
  }
}
