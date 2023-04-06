using Logitar.Portal.v2.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.v2.Core.Realms.Queries;

internal record GetRealm(string IdOrUniqueName) : IRequest<Realm?>;
