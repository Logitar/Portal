﻿using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Domain.Senders;

public class SenderProviderNotSupportedException : NotSupportedException
{
  private const string ErrorMessage = "The specified sender provider is not supported.";

  public SenderProvider SenderProvider
  {
    get => (SenderProvider)Data[nameof(SenderProvider)]!;
    private set => Data[nameof(SenderProvider)] = value;
  }

  public SenderProviderNotSupportedException(SenderProvider senderProvider)
    : base(BuildMessage(senderProvider))
  {
    SenderProvider = senderProvider;
  }

  private static string BuildMessage(SenderProvider senderProvider) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(SenderProvider), senderProvider)
    .Build();
}
