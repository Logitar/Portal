using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Queries
{
  internal record GetApiKeysQuery(DateTime? ExpiredOn, string? Search,
    ApiKeySort? Sort, bool IsDescending, int? Index, int? Count) : IRequest<ListModel<ApiKeyModel>>;
}
