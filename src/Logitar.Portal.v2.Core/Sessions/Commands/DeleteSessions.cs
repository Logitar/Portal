using Logitar.Portal.v2.Core.Realms;
using Logitar.Portal.v2.Core.Users;
using MediatR;

namespace Logitar.Portal.v2.Core.Sessions.Commands
{
  internal record DeleteSessions : IRequest
  {
    public DeleteSessions(RealmAggregate realm) => Realm = realm;
    public DeleteSessions(UserAggregate user) => User = user;

    public RealmAggregate? Realm { get; }
    public UserAggregate? User { get; }
  }
}
