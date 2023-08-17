using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

public record UpdateRealmCommand(string Id, UpdateRealmPayload Payload) : IRequest<Realm?>;
