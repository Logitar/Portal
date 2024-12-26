using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands;

internal record ReplaceSenderCommand(Guid Id, ReplaceSenderPayload Payload, long? Version) : Activity, IRequest<SenderModel?>;
