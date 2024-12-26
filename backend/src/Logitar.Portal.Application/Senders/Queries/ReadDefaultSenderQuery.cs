using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Queries;

internal record ReadDefaultSenderQuery : Activity, IRequest<SenderModel?>;
