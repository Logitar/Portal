using FluentValidation;

namespace Portal.Core.Emails.Senders
{
  internal class SenderValidator : AbstractValidator<Sender>
  {
    public SenderValidator()
    {
      RuleFor(x => x.EmailAddress)
        .NotEmpty()
        .MaximumLength(256)
        .EmailAddress();

      RuleFor(x => x.DisplayName)
        .MaximumLength(256);

      RuleForEach(x => x.Settings.Keys)
        .NotEmpty()
        .MaximumLength(256)
        .Must(ValidationRules.BeAValidIdentifier);
    }
  }
}
