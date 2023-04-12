using Logitar.Portal.v2.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.v2.Core.Realms.Commands;

internal record CreateRealm(CreateRealmInput Input) : IRequest<Realm>;
