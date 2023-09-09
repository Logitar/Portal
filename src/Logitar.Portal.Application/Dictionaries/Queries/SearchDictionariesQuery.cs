using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Queries;

internal record SearchDictionariesQuery(SearchDictionariesPayload Payload) : IRequest<SearchResults<Dictionary>>;
