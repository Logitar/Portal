using Logitar.Portal.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands;

internal record CreateDictionaryCommand(CreateDictionaryPayload Payload) : IRequest<Dictionary>;
