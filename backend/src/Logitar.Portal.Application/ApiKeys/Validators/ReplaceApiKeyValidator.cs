using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.ApiKeys;

namespace Logitar.Portal.Application.ApiKeys.Validators;

internal class ReplaceApiKeyValidator : AbstractValidator<ReplaceApiKeyPayload>
{
  public ReplaceApiKeyValidator()
  {
    RuleFor(x => x.DisplayName).DisplayName();
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());
    When(x => x.ExpiresOn.HasValue, () => RuleFor(x => x.ExpiresOn!.Value).Future()); // TODO(fpion): handle API key exceptions

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeContractValidator());
  }
}
