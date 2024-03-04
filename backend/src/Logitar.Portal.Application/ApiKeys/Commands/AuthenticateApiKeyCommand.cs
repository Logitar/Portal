using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal record AuthenticateApiKeyCommand(AuthenticateApiKeyPayload Payload) : ApplicationRequest, IRequest<ApiKey>;
