using FluentValidation;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Domain.Senders.SendGrid;

public class SendGridSettingsValidator : AbstractValidator<ISendGridSettings>
{
  public SendGridSettingsValidator()
  {
    RuleFor(x => x.ApiKey).NotEmpty();
  }
}
