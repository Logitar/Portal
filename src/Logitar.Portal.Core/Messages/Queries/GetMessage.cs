using Logitar.Portal.Contracts.Messages;
using MediatR;

namespace Logitar.Portal.Core.Messages.Queries;

internal record GetMessage(Guid? Id) : IRequest<Message?>;
