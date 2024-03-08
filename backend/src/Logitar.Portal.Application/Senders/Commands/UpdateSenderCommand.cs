using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands;

internal record UpdateSenderCommand(Guid Id, UpdateSenderPayload Payload) : Activity, IRequest<Sender?>;
