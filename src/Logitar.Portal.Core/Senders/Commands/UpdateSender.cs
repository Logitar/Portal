using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Core.Senders.Commands;

internal record UpdateSender(Guid Id, UpdateSenderInput Input) : IRequest<Sender>;
