using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Queries;

internal record SearchApiKeysQuery(SearchApiKeysPayload Payload) : IRequest<SearchResults<ApiKey>>;
