using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Queries;

internal record ReadApiKeyQuery(string? Id) : IRequest<ApiKey?>;
