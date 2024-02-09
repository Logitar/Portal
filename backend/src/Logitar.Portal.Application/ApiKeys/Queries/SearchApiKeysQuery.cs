using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Queries;

internal record SearchApiKeysQuery(SearchApiKeysPayload Payload) : IRequest<SearchResults<ApiKey>>;
