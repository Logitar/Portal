using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Application.Messages.Validators;

internal class VariableValidator : AbstractValidator<Variable>
{
  public VariableValidator()
  {
    RuleFor(x => x.Key).SetValidator(new IdentifierValidator());
    RuleFor(x => x.Value).NotEmpty();
  }
}
