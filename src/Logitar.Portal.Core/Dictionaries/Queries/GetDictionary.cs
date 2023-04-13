using Logitar.Portal.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.Core.Dictionaries.Queries;

internal record GetDictionary(Guid? Id) : IRequest<Dictionary?>;
