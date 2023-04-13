using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using MediatR;

namespace Logitar.Portal.Core.Messages.Queries;

internal record GetMessages(bool? HasErrors, bool? IsDemo, string? Realm, string? Search, bool? Succeeded, string? Template,
    MessageSort? Sort, bool IsDescending, int? Skip, int? Limit) : IRequest<PagedList<Message>>;
