using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Core.Templates.Queries;

internal record GetTemplate(Guid? Id, string? Realm, string? Key) : IRequest<Template?>;
