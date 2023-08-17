using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Queries;

public record SearchRealmsQuery(SearchRealmsPayload Payload) : IRequest<SearchResults<Realm>>;
