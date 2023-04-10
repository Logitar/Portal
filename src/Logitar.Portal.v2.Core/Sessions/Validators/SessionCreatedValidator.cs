using FluentValidation;
using Logitar.Portal.v2.Core.Sessions.Events;

namespace Logitar.Portal.v2.Core.Sessions.Validators;

internal class SessionCreatedValidator : AbstractValidator<SessionCreated>
{
  public SessionCreatedValidator()
  {
    RuleForEach(x => x.CustomAttributes.Keys).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier();
    RuleForEach(x => x.CustomAttributes.Values).NotEmpty();
  }
}
