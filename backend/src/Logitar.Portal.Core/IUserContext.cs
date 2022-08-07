using Logitar.Portal.Core.Actors;

namespace Logitar.Portal.Core
{
  public interface IUserContext
  {
    Actor Actor { get; }

    Guid Id { get; }
    Guid SessionId { get; }

    string BaseUrl { get; }
  }
}
