using FluentValidation;

namespace Portal.Core.Emails.Templates
{
  internal class TemplateValidator : AbstractValidator<Template>
  {
    public TemplateValidator()
    {
      RuleFor(x => x.Key)
        .NotEmpty()
        .MaximumLength(256)
        .Must(BeAValidKey);

      RuleFor(x => x.DisplayName)
        .MaximumLength(256);
    }

    private static bool BeAValidKey(string? value) => value == null
      || value.All(c => char.IsLetterOrDigit(c) || c == '_');
  }
}
