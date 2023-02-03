using FluentValidation;
using Logitar.Portal.Domain.Templates;
using System.Net.Mime;

namespace Logitar.Portal.Application.Templates
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
      RuleFor(x => x.Key).NotEmpty()
        .MaximumLength(256)
        .Identifier();

      RuleFor(x => x.DisplayName).NullOrNotEmpty()
        .MaximumLength(256);

      RuleFor(x => x.Description).NullOrNotEmpty();

      RuleFor(x => x.Subject).NotEmpty()
        .MaximumLength(256);

      RuleFor(x => x.ContentType).NotEmpty()
        .Must(_allowedContentTypes.Contains)
        .WithErrorCode("ContentTypeValidator")
        .WithMessage($"'{{PropertyName}}' must be one of the following: {string.Join(", ", _allowedContentTypes)}");

      RuleFor(x => x.Contents).NotEmpty();
    }
  }
}
