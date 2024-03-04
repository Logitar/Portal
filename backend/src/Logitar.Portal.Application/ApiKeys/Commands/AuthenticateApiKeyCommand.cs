using Logitar.Portal.Application.Logging;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal record AuthenticateApiKeyCommand(AuthenticateApiKeyPayload Payload) : ApplicationRequest, IRequest<ApiKey>
{
  public override IActivity GetActivity()
  {
    AuthenticateApiKeyCommand command = this.DeepClone();
    command.Payload.XApiKey = Payload.XApiKey.Mask();
    return command;
  }
}
