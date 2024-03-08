using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands;

internal record UpdateDictionaryCommand(Guid Id, UpdateDictionaryPayload Payload) : Activity, IRequest<Dictionary?>;
