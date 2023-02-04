using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands
{
  internal record UpdateApiKeyCommand(string Id, UpdateApiKeyPayload Payload) : IRequest<ApiKeyModel>;
}
