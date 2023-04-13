using FluentValidation;
using Logitar.Portal.Core.Senders.Events;

namespace Logitar.Portal.Core.Senders.Validators;

internal class SenderCreatedValidator : SenderSavedValidator<SenderCreated>
{
  public SenderCreatedValidator() : base()
  {
    RuleFor(x => x.Provider).IsInEnum();
  }
}
