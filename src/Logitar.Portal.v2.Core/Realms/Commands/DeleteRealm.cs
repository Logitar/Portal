using Logitar.Portal.v2.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.v2.Core.Realms.Commands;

internal record DeleteRealm(Guid Id) : IRequest<Realm>;
