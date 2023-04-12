using Logitar.Portal.v2.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.v2.Core.Dictionaries.Queries;

internal record GetDictionary(Guid? Id) : IRequest<Dictionary?>;
