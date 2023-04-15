using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.Core.Dictionaries.Queries;

internal record GetDictionaries(string? Locale, string? Realm, DictionarySort? Sort,
  bool IsDescending, int? Skip, int? Limit) : IRequest<PagedList<Dictionary>>;
