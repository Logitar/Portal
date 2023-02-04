using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Queries
{
  internal record GetApiKeyQuery(string Id) : IRequest<ApiKeyModel?>;
}
