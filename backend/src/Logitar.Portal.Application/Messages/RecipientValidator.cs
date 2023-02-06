using FluentValidation;
using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.Application.Messages
{
  internal class RecipientValidator : AbstractValidator<Recipient>
  {
    public RecipientValidator()
    {
      RuleFor(x => x.Type).IsInEnum();

      RuleFor(x => x.Address).NotEmpty()
        .MaximumLength(255)
        .EmailAddress();

      RuleFor(x => x.DisplayName).NullOrNotEmpty()
        .MaximumLength(383);

      When(x => x.UserId.HasValue, () =>
      {
        RuleFor(x => x.Username).NullOrNotEmpty()
          .MaximumLength(255);

        RuleFor(x => x.UserLocale).Locale();
      }).Otherwise(() =>
      {
        RuleFor(x => x.Username).Null();

        RuleFor(x => x.UserLocale).Null();
      });
    }
  }
}
