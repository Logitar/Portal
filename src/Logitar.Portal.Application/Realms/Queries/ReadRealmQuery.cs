using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Queries;

public record ReadRealmQuery(string? Id, string? UniqueSlug) : IRequest<Realm?>;
