using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal record UpdateApiKeyCommand(Guid Id, UpdateApiKeyPayload Payload) : Activity, IRequest<ApiKeyModel?>;
