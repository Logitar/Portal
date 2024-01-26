using FluentValidation;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.Configurations;

namespace Logitar.Portal.Application.Configurations.Validators;

internal class SessionPayloadValidator : AbstractValidator<SessionPayload>
{
  public SessionPayloadValidator()
  {
    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeContractValidator());
  }
}
