using Logitar.Portal.Domain.Actors;

namespace Logitar.Portal.Application
{
  public interface IUserContext
  {
    Actor Actor { get; }

    Guid Id { get; }
    Guid SessionId { get; }

    string BaseUrl { get; }
  }
}
