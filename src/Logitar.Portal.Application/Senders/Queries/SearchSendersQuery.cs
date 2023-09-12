using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Queries;

internal record SearchSendersQuery(SearchSendersPayload Payload) : IRequest<SearchResults<Sender>>;
