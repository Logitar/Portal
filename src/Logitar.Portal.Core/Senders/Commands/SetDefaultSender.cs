using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Core.Senders.Commands;

internal record SetDefaultSender(Guid Id) : IRequest<Sender>;
