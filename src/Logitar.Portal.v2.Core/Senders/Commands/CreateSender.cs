using Logitar.Portal.v2.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.v2.Core.Senders.Commands;

internal record CreateSender(CreateSenderInput Input) : IRequest<Sender>;
