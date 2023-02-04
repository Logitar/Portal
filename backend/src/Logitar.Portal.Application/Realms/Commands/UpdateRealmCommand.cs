using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands
{
  internal record UpdateRealmCommand(string Id, UpdateRealmPayload Payload) : IRequest<RealmModel>;
}
