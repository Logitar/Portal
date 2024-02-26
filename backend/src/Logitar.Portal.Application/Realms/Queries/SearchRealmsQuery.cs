using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Logitar.Portal.Application.Realms.Queries;

internal record SearchRealmsQuery(SearchRealmsPayload Payload) : IRequest<SearchResults<Realm>>;
