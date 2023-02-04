using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Queries
{
  internal record GetRealmQuery(string Id) : IRequest<RealmModel>;
}
