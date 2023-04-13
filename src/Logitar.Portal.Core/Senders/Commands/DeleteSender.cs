using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Core.Senders.Commands;

internal record DeleteSender(Guid Id) : IRequest<Sender>;
