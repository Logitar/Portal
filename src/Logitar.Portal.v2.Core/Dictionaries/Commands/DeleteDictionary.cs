using Logitar.Portal.v2.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.v2.Core.Dictionaries.Commands;

internal record DeleteDictionary(Guid Id) : IRequest<Dictionary>;
