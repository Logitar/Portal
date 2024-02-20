using FluentValidation;

namespace Logitar.Portal.Domain.Messages.Validators;

internal class RecipientValidator : AbstractValidator<RecipientUnit>
{
  public RecipientValidator()
  {
    RuleFor(x => x.Type).IsInEnum();

    RuleFor(x => x.Address).NotEmpty().EmailAddress();
    When(x => x.DisplayName != null, () => RuleFor(x => x.DisplayName).NotEmpty());

    When(x => x.User != null, () => RuleFor(x => x.UserId).NotNull());
  }
}
