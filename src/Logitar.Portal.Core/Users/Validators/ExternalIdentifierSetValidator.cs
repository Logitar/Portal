using FluentValidation;
using Logitar.Portal.Core.Users.Events;

namespace Logitar.Portal.Core.Users.Validators;

internal class ExternalIdentifierSetValidator : AbstractValidator<ExternalIdentifierSet>
{
  public ExternalIdentifierSetValidator()
  {
    RuleFor(x => x.Key).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier();

    RuleFor(x => x.Value).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);
  }
}
