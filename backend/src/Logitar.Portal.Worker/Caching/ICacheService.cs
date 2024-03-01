using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.MassTransit.Caching;

internal interface ICacheService
{
  SentMessages? GetSentMessages(Guid correlationId);
  void SetSentMessages(Guid correlationId, SentMessages sentMessages);
}
