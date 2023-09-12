using Logitar.Portal.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Queries;

internal record ReadDictionaryQuery(Guid? Id, string? Realm, string? Locale) : IRequest<Dictionary?>;
