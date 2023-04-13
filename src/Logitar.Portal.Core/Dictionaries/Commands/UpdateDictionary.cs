using Logitar.Portal.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.Core.Dictionaries.Commands;

internal record UpdateDictionary(Guid Id, UpdateDictionaryInput Input) : IRequest<Dictionary>;
