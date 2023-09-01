using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal record AuthenticateApiKeyCommand(string XApiKey) : IRequest<ApiKey>;
