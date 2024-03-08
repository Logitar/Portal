using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

internal record CreateRealmCommand(CreateRealmPayload Payload) : Activity, IRequest<Realm>;
