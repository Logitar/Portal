using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.MassTransitDemo.Worker.Caching;

public interface ICacheService
{
  SentMessages? GetSentMessages(Guid correlationId);
  void SetSentMessages(Guid correlationId, SentMessages sentMessages);
}
