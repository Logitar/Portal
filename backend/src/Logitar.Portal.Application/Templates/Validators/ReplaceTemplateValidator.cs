using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Templates.Validators;

namespace Logitar.Portal.Application.Templates.Validators;

internal class ReplaceTemplateValidator : AbstractValidator<ReplaceTemplatePayload>
{
  public ReplaceTemplateValidator()
  {
    RuleFor(x => x.UniqueKey).Identifier();
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x.Subject).Subject();
    RuleFor(x => x.Content).SetValidator(new ContentValidator());
  }
}
