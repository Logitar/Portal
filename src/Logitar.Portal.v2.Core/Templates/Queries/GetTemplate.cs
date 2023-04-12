using Logitar.Portal.v2.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.v2.Core.Templates.Queries;

internal record GetTemplate(Guid? Id, string? Realm, string? Key) : IRequest<Template?>;
