using Logitar.Portal.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.Core.Dictionaries.Commands;

internal record DeleteDictionary(Guid Id) : IRequest<Dictionary>;
