using FluentValidation;
using Logitar.Portal.v2.Core.Templates.Events;
using System.Net.Mime;

namespace Logitar.Portal.v2.Core.Templates.Validators;

internal abstract class TemplateSavedValidator<T> : AbstractValidator<T> where T : TemplateSaved
{
  private static readonly HashSet<string> _allowedContentTypes = new(new[]
  {
    MediaTypeNames.Text.Plain,
    MediaTypeNames.Text.Html
  });

  protected TemplateSavedValidator()
  {
    RuleFor(x => x.DisplayName).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.Description).NullOrNotEmpty();

    RuleFor(x => x.Subject).NotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleFor(x => x.ContentType).NotEmpty()
      .Must(_allowedContentTypes.Contains).WithErrorCode("ContentTypeValidator")
      .WithMessage($"'{{PropertyName}}' may only be one of the following: {string.Join(", ", _allowedContentTypes)}");

    RuleFor(x => x.Contents).NullOrNotEmpty();
  }
}
