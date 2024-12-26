using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Logitar.Portal.Application.Messages.Queries;

internal record SearchMessagesQuery(SearchMessagesPayload Payload) : Activity, IRequest<SearchResults<MessageModel>>;
