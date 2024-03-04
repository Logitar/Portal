using Logitar.Portal.Contracts.Messages;
using MediatR;

namespace Logitar.Portal.Application.Messages.Queries;

internal record ReadMessageQuery(Guid Id) : ApplicationRequest, IRequest<Message>;
