using FluentValidation;
using Logitar.Portal.Domain.Sessions;

namespace Logitar.Portal.Application.Sessions
{
  internal class SessionValidator : AbstractValidator<Session>
  {
    public SessionValidator()
    {
      RuleFor(x => x.KeyHash).NullOrNotEmpty();

      RuleFor(x => x.IpAddress).NullOrNotEmpty()
        .MaximumLength(64);

      RuleFor(x => x.AdditionalInformation).NullOrNotEmpty();
    }
  }
}
