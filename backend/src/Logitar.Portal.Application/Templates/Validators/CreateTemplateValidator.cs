using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Templates.Validators;

namespace Logitar.Portal.Application.Templates.Validators;

internal class CreateTemplateValidator : AbstractValidator<CreateTemplatePayload>
{
  public CreateTemplateValidator()
  {
    When(x => x.Id.HasValue, () => RuleFor(x => x.Id!.Value).NotEmpty());

    RuleFor(x => x.UniqueKey).Identifier();
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleFor(x => x.Subject).Subject();
    RuleFor(x => x.Content).SetValidator(new ContentValidator());
  }
}
