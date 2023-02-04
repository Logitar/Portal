using FluentValidation;
using Logitar.Portal.Domain.Templates;

namespace Logitar.Portal.Application.Templates
{
  internal class TemplateValidator : AbstractValidator<Template>
  {
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
        .ContentType();

      RuleFor(x => x.Contents).NotEmpty();
    }
  }
}
