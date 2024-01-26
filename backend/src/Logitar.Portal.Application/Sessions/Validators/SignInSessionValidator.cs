using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Application.Sessions.Validators;

internal class SignInSessionValidator : AbstractValidator<SignInSessionPayload>
{
  public SignInSessionValidator()
  {
    When(x => x.Id != null, () => RuleFor(x => x.Id!).SetValidator(new IdValidator()));

    RuleFor(x => x.UniqueName).NotEmpty();
    RuleFor(x => x.Password).NotEmpty();

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeContractValidator());
  }
}
