using Logitar.Portal.Core.Actors.Models;

namespace Logitar.Portal.Core
{
  internal interface IUserContext
  {
    ActorModel Actor { get; }
    AggregateId ActorId { get; }

    public string Id { get; }
    string SessionId { get; }

    string BaseUrl { get; }
  }
}
