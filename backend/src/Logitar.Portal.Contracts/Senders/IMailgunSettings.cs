﻿namespace Logitar.Portal.Contracts.Senders;

public interface IMailgunSettings
{
  string ApiKey { get; }
  string DomainName { get; }
}
