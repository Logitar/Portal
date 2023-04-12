using Logitar.Portal.v2.Core.Realms;
using MediatR;

namespace Logitar.Portal.v2.Core.Users.Commands;

internal record DeleteUsers(RealmAggregate Realm) : IRequest;
