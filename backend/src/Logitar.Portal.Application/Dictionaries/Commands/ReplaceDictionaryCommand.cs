using Logitar.Portal.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands;

internal record ReplaceDictionaryCommand(Guid Id, ReplaceDictionaryPayload Payload, long? Version) : Activity, IRequest<Dictionary?>;
