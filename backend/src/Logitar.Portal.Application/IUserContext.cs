using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Domain;

namespace Logitar.Portal.Application
{
  public interface IUserContext
  {
    ActorModel Actor { get; }
    AggregateId ActorId { get; }

    string SessionId { get; }
    string UserId { get; }

    string BaseUrl { get; }
  }
}
