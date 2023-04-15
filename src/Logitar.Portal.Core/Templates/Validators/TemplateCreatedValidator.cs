using FluentValidation;
using Logitar.Portal.Core.Templates.Events;

namespace Logitar.Portal.Core.Templates.Validators;

internal class TemplateCreatedValidator : TemplateSavedValidator<TemplateCreated>
{
  public TemplateCreatedValidator() : base()
  {
    RuleFor(x => x.UniqueName).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier();
  }
}
