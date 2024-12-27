using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Portal.Contracts.Templates;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Templates.Validators;

namespace Logitar.Portal.Application.Templates.Validators;

internal class UpdateTemplateValidator : AbstractValidator<UpdateTemplatePayload>
{
  public UpdateTemplateValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueKey), () => RuleFor(x => x.UniqueKey!).Identifier());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => !string.IsNullOrWhiteSpace(x.Subject), () => RuleFor(x => x.Subject!).Subject());
    When(x => x.Content != null, () => RuleFor(x => x.Content!).SetValidator(new ContentValidator()));
  }
}
