using Logitar.Portal.Domain;

namespace Logitar.Portal.Application
{
  public interface IUserContext
  {
    AggregateId ActorId { get; }

    string SessionId { get; }
    string UserId { get; }
  }
}
