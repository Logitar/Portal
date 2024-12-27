using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Portal.Application.Tokens.Validators;

internal class ClaimValidator : AbstractValidator<ClaimModel>
{
  public ClaimValidator()
  {
    RuleFor(x => x.Name).SetValidator(new IdentifierValidator());
    RuleFor(x => x.Value).NotEmpty();
  }
}
