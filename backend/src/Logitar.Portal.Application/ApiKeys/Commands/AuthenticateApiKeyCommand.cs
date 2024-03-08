using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal record AuthenticateApiKeyCommand(AuthenticateApiKeyPayload Payload) : Activity, IRequest<ApiKey>
{
  public override IActivity Anonymize()
  {
    AuthenticateApiKeyCommand command = this.DeepClone();
    command.Payload.XApiKey = Payload.XApiKey.Mask();
    return command;
  }
}
