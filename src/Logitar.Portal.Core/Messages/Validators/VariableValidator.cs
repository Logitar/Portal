using FluentValidation;
using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Core.Messages.Validators;

internal class VariableValidator : AbstractValidator<Variable>
{
  public VariableValidator()
  {
    RuleFor(x => x.Key).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier();

    RuleFor(x => x.Value).NotEmpty();
  }
}
