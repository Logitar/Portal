using Logitar.Portal.v2.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.v2.Core.Senders.Queries;

internal record GetSender(Guid? Id, string? DefaultInRealm) : IRequest<Sender?>;
