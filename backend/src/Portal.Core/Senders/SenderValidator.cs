using FluentValidation;

namespace Portal.Core.Senders
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
        .Must(BeAValidIdentifier);
    }

    private static bool BeAValidIdentifier(string? value) => string.IsNullOrEmpty(value)
      || (!char.IsDigit(value.First()) && value.All(c => char.IsLetterOrDigit(c) || c == '_'));
  }
}
