using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain.Templates.Validators;

namespace Logitar.Portal.Application.Templates.Validators;

internal class ReplaceTemplateValidator : AbstractValidator<ReplaceTemplatePayload>
{
  public ReplaceTemplateValidator()
  {
    RuleFor(x => x.UniqueKey).SetValidator(new UniqueKeyValidator());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));

    RuleFor(x => x.Subject).SetValidator(new SubjectValidator());
    RuleFor(x => x.Content).SetValidator(new ContentValidator());
  }
}
