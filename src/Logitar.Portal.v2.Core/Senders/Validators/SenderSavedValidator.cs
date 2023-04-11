using FluentValidation;
using Logitar.Portal.v2.Core.Senders.Events;

namespace Logitar.Portal.v2.Core.Senders.Validators;

internal class SenderSavedValidator<T> : AbstractValidator<T> where T : SenderSaved
{
  protected SenderSavedValidator()
  {
    RuleFor(x => x.EmailAddress).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .EmailAddress();

    RuleFor(x => x.DisplayName).NullOrNotEmpty()
      .MaximumLength(byte.MaxValue);

    RuleForEach(x => x.Settings.Keys).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier();
    RuleForEach(x => x.Settings.Values).NotEmpty();
  }
}
