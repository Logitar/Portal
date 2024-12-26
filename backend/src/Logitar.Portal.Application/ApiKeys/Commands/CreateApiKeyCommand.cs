using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal record CreateApiKeyCommand(CreateApiKeyPayload Payload) : Activity, IRequest<ApiKeyModel>;
