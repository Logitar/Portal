﻿using FluentValidation;

namespace Logitar.Portal.v2.Infrastructure.Messages.Providers.SendGrid;

internal class SendGridStrategy : ISendGridStrategy
{
  public IMessageHandler Execute(IReadOnlyDictionary<string, string> settings)
  {
    SendGridSettings providerSettings = new(settings);
    new SendGridValidator().ValidateAndThrow(providerSettings);

    return new SendGridHandler(providerSettings);
  }
}