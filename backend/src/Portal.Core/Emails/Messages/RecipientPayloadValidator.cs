using FluentValidation;
using Portal.Core.Emails.Messages.Payloads;

namespace Portal.Core.Emails.Messages
{
  internal class RecipientPayloadValidator : AbstractValidator<RecipientPayload>
  {
    public RecipientPayloadValidator()
    {
      When(x => x.Type == RecipientType.To, () =>
      {
        RuleFor(x => x.UserId).NotNull();
        RuleFor(x => x.Address).Null();
        RuleFor(x => x.DisplayName).Null();
      });

      When(x => x.Type != RecipientType.To, () =>
      {
        When(x => x.UserId.HasValue, () =>
        {
          RuleFor(x => x.Address).Null();
          RuleFor(x => x.DisplayName).Null();
        });

        When(x => !x.UserId.HasValue, () =>
        {
          RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(256)
            .EmailAddress();

          RuleFor(x => x.DisplayName)
            .MaximumLength(256);
        });
      });
    }
  }
}
