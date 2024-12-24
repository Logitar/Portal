using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Application.Messages.Validators;

internal class VariableValidator : AbstractValidator<Variable>
{
  public VariableValidator()
  {
    RuleFor(x => x.Key).Identifier();
    RuleFor(x => x.Value).NotEmpty();
  }
}
