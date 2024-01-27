using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Shared.Validators;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.ApiKeys;

namespace Logitar.Portal.Application.ApiKeys.Validators;

internal class CreateApiKeyValidator : AbstractValidator<CreateApiKeyPayload>
{
  public CreateApiKeyValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Id), () => RuleFor(x => x.Id!).SetValidator(new IdValidator()));

    RuleFor(x => x.DisplayName).SetValidator(new DisplayNameValidator());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));
    When(x => x.ExpiresOn.HasValue, () => RuleFor(x => x.ExpiresOn!.Value).SetValidator(new ExpirationValidator()));

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeContractValidator());

    RuleForEach(x => x.Roles).NotEmpty();
  }
}
