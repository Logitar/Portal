using FluentValidation;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.Passwords;

namespace Logitar.Portal.Application.OneTimePasswords.Validators;

internal class ValidateOneTimePasswordValidator : AbstractValidator<ValidateOneTimePasswordPayload>
{
  public ValidateOneTimePasswordValidator()
  {
    RuleFor(x => x.Password).NotEmpty();

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeContractValidator());
  }
}
