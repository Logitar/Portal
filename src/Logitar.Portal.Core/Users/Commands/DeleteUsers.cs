using Logitar.Portal.Core.Realms;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands;

internal record DeleteUsers(RealmAggregate Realm) : IRequest;
