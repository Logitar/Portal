using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Queries;

internal record ReadRealmQuery(string? Id, string? UniqueSlug) : IRequest<Realm?>;
