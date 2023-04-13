using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Core.Realms.Commands;

internal record UpdateRealm(Guid Id, UpdateRealmInput Input) : IRequest<Realm>;
