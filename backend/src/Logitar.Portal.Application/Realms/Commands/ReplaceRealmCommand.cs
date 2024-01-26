using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

internal record ReplaceRealmCommand(string Id, ReplaceRealmPayload Payload, long? Version) : IRequest<Realm?>;
