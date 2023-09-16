using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using MediatR;

namespace Logitar.Portal.Application.Messages.Queries;

internal record SearchMessagesQuery(SearchMessagesPayload Payload) : IRequest<SearchResults<Message>>;
