using FluentValidation;
using System.Net.Mime;

namespace Logitar.Portal.Core.Emails.Templates
{
  internal class TemplateValidator : AbstractValidator<Template>
  {
    private static readonly HashSet<string> _allowedContentTypes = new(new[]
    {
      MediaTypeNames.Text.Plain,
      MediaTypeNames.Text.Html
    });

    public TemplateValidator()
    {
      RuleFor(x => x.Key)
        .NotEmpty()
        .MaximumLength(256)
        .Must(BeAValidKey);

      RuleFor(x => x.ContentType)
        .NotEmpty()
        .Must(_allowedContentTypes.Contains);

      RuleFor(x => x.DisplayName)
        .MaximumLength(256);
    }

    private static bool BeAValidKey(string? value) => value == null
      || value.All(c => char.IsLetterOrDigit(c) || c == '_');
  }
}
