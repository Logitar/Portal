using FluentValidation;
using Logitar.Portal.v2.Core.Sessions.Events;

namespace Logitar.Portal.v2.Core.Sessions.Validators;

internal class SessionSavedValidator<T> : AbstractValidator<T> where T : SessionSaved
{
  protected SessionSavedValidator()
  {
    RuleForEach(x => x.CustomAttributes.Keys).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier();
    RuleForEach(x => x.CustomAttributes.Values).NotEmpty();
  }
}
