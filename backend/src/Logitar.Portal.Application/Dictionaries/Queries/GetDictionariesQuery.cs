using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Application.Dictionaries.Queries
{
  internal record GetDictionariesQuery(CultureInfo? Locale, string? Realm,
    DictionarySort? Sort, bool IsDescending, int? Index, int? Count) : IRequest<ListModel<DictionaryModel>>;
}
