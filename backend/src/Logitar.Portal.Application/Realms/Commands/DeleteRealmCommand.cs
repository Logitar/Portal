using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

internal record DeleteRealmCommand(Guid Id) : ApplicationRequest, IRequest<Realm?>;
