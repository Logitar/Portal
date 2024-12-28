using FluentValidation;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Application.Sessions.Validators;

internal class CreateSessionValidator : AbstractValidator<CreateSessionPayload>
{
  public CreateSessionValidator()
  {
    When(x => x.Id.HasValue, () => RuleFor(x => x.Id!.Value).NotEmpty());

    RuleFor(x => x.User).NotEmpty();

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeContractValidator());
  }
}
