using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

internal record DeleteSessionsCommand : INotification
{
  public DeleteSessionsCommand(RealmAggregate realm)
  {
    Realm = realm;
  }
  public DeleteSessionsCommand(UserAggregate user)
  {
    User = user;
  }

  public RealmAggregate? Realm { get; }
  public UserAggregate? User { get; }
}
