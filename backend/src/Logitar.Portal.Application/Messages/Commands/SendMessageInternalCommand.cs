using Logitar.Portal.Contracts.Messages;
using MediatR;

namespace Logitar.Portal.Application.Messages.Commands;

internal record SendMessageInternalCommand(SendMessagePayload Payload) : IRequest<SentMessages>;
