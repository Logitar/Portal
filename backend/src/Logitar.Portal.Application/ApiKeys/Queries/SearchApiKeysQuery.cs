using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Queries;

internal record SearchApiKeysQuery(SearchApiKeysPayload Payload) : Activity, IRequest<SearchResults<ApiKeyModel>>;
