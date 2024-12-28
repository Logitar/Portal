using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.ApiKeys;

namespace Logitar.Portal.Application.ApiKeys.Validators;

internal class CreateApiKeyValidator : AbstractValidator<CreateApiKeyPayload>
{
  public CreateApiKeyValidator()
  {
    When(x => x.Id.HasValue, () => RuleFor(x => x.Id!.Value).NotEmpty());

    RuleFor(x => x.DisplayName).DisplayName();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
    When(x => x.ExpiresOn.HasValue, () => RuleFor(x => x.ExpiresOn!.Value).Future());

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeContractValidator());
  }
}
