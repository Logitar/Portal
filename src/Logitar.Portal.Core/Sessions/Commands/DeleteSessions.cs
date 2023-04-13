using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Users;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Commands
{
  internal record DeleteSessions : IRequest
  {
    public DeleteSessions(RealmAggregate realm) => Realm = realm;
    public DeleteSessions(UserAggregate user) => User = user;

    public RealmAggregate? Realm { get; }
    public UserAggregate? User { get; }
  }
}
