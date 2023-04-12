using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Messages;
using MediatR;

namespace Logitar.Portal.v2.Core.Messages.Queries;

internal record GetMessages(bool? HasErrors, bool? IsDemo, string? Realm, string? Search, bool? Succeeded, string? Template,
    MessageSort? Sort, bool IsDescending, int? Skip, int? Limit) : IRequest<PagedList<Message>>;
