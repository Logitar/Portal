using FluentValidation;
using Logitar.Portal.Core.Emails.Messages;
using Logitar.Portal.Core.Emails.Providers;

namespace Logitar.Portal.Infrastructure.Emails.Providers.SendGrid
{
  internal class SendGridStrategy : ISendGridStrategy
  {
    private readonly IValidator<SendGridSettings> _validator;

    public SendGridStrategy(IValidator<SendGridSettings> validator)
    {
      _validator = validator;
    }

    public IMessageHandler Execute(IReadOnlyDictionary<string, string?> settings)
    {
      var providerSettings = new SendGridSettings(settings);

      _validator.Validate(providerSettings);

      return new SendGridHandler(providerSettings);
    }
  }
}
