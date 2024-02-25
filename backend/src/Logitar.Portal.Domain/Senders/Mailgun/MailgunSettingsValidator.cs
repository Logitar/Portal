using FluentValidation;
using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Domain.Senders.Mailgun;

public class MailgunSettingsValidator : AbstractValidator<IMailgunSettings>
{
  public MailgunSettingsValidator()
  {
    RuleFor(x => x.ApiKey).NotEmpty();
    RuleFor(x => x.DomainName).NotEmpty();
  }
}
