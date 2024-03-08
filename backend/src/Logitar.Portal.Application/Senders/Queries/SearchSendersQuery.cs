using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Queries;

internal record SearchSendersQuery(SearchSendersPayload Payload) : Activity, IRequest<SearchResults<Sender>>;
