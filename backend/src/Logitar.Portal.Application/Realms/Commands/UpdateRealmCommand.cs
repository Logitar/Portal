using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

internal record UpdateRealmCommand(Guid Id, UpdateRealmPayload Payload) : ApplicationRequest, IRequest<Realm?>;
