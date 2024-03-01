using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Queries;

public record ReadRealmQuery(Guid? Id, string? UniqueSlug) : IRequest<Realm?>;
