using FluentValidation;
using Logitar.Portal.v2.Core.Templates.Events;

namespace Logitar.Portal.v2.Core.Templates.Validators;

internal class TemplateCreatedValidator : TemplateSavedValidator<TemplateCreated>
{
  public TemplateCreatedValidator() : base()
  {
    RuleFor(x => x.UniqueName).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier();
  }
}
