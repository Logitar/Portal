using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Queries;

internal record SearchDictionariesQuery(SearchDictionariesPayload Payload) : ApplicationRequest, IRequest<SearchResults<Dictionary>>;
