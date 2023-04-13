using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Core.Realms.Queries;

internal record GetRealm(Guid? Id, string? UniqueName) : IRequest<Realm?>;
