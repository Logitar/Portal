using Logitar.Portal.v2.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.v2.Core.Realms.Commands;

internal record UpdateRealm(Guid Id, UpdateRealmInput Input) : IRequest<Realm>;
