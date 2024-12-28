using FluentValidation;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Application.Sessions.Validators;

internal class SignInSessionValidator : AbstractValidator<SignInSessionPayload>
{
  public SignInSessionValidator()
  {
    When(x => x.Id.HasValue, () => RuleFor(x => x.Id!.Value).NotEmpty());

    RuleFor(x => x.UniqueName).NotEmpty();
    RuleFor(x => x.Password).NotEmpty();

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeContractValidator());
  }
}
