using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

public record DeleteRealmCommand(string Id) : IRequest<Realm?>;
