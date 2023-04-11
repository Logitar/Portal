using Logitar.Portal.v2.Contracts.Messages;
using MediatR;

namespace Logitar.Portal.v2.Core.Messages.Queries;

internal record GetMessage(Guid? Id) : IRequest<Message?>;
