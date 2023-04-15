using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Core.Senders.Queries;

internal record GetSender(Guid? Id, string? DefaultInRealm) : IRequest<Sender?>;
