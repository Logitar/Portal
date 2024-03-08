using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Queries;

internal record ReadSenderQuery(Guid Id) : Activity, IRequest<Sender?>;
