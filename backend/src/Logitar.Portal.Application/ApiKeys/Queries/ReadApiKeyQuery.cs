using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Queries;

internal record ReadApiKeyQuery(Guid Id) : Activity, IRequest<ApiKey?>;
