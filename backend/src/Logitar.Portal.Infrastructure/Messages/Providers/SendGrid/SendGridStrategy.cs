using FluentValidation;
using Logitar.Portal.Application.Messages;
using Logitar.Portal.Application.Messages.Providers;

namespace Logitar.Portal.Infrastructure.Messages.Providers.SendGrid
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
      SendGridSettings providerSettings = new(settings);

      _validator.Validate(providerSettings);

      return new SendGridHandler(providerSettings);
    }
  }
}
