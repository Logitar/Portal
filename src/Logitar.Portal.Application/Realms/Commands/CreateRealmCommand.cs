using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

public record CreateRealmCommand(CreateRealmPayload Payload) : IRequest<Realm>;
