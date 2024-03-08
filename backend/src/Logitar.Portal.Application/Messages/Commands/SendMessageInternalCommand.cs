using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Messages;
using MediatR;

namespace Logitar.Portal.Application.Messages.Commands;

public record SendMessageInternalCommand(SendMessagePayload Payload) : Activity, IRequest<SentMessages>;
