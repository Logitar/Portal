using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

public record ReplaceRealmCommand(string Id, ReplaceRealmPayload Payload, long? Version) : IRequest<Realm?>;
