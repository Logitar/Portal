using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

internal record ReplaceRealmCommand(Guid Id, ReplaceRealmPayload Payload, long? Version) : ApplicationRequest, IRequest<Realm?>;
