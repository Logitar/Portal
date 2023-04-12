using FluentValidation;
using Logitar.Portal.v2.Contracts.Messages;

namespace Logitar.Portal.v2.Core.Messages.Validators;

internal class RecipientInputValidator : AbstractValidator<RecipientInput>
{
  public RecipientInputValidator()
  {
    When(x => x.User == null, () =>
    {
      RuleFor(x => x.Address).NotEmpty()
        .MaximumLength(byte.MaxValue)
        .EmailAddress();

      RuleFor(x => x.DisplayName).MaximumLength(byte.MaxValue);
    }).Otherwise(() =>
    {
      RuleFor(x => x.Address).Null();
      RuleFor(x => x.DisplayName).Null();
    });
  }
}
