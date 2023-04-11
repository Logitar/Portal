using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.v2.Core.Dictionaries.Queries;

internal record GetDictionaries(string? Locale, string? Realm, DictionarySort? Sort,
  bool IsDescending, int? Skip, int? Limit) : IRequest<PagedList<Dictionary>>;
