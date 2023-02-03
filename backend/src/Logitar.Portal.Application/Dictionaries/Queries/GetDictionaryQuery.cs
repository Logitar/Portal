using Logitar.Portal.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Queries
{
  internal record GetDictionaryQuery(string Id) : IRequest<DictionaryModel?>;
}
