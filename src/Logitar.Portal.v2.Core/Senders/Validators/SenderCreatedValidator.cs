using FluentValidation;
using Logitar.Portal.v2.Core.Senders.Events;

namespace Logitar.Portal.v2.Core.Senders.Validators;

internal class SenderCreatedValidator : SenderSavedValidator<SenderCreated>
{
  public SenderCreatedValidator() : base()
  {
    RuleFor(x => x.Provider).IsInEnum();
  }
}
