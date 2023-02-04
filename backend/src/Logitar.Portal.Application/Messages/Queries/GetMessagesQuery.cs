using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using MediatR;

namespace Logitar.Portal.Application.Messages.Queries
{
  internal record GetMessagesQuery(bool? HasErrors, bool? HasSucceeded, bool? IsDemo, string? Realm, string? Search, string? Template,
    MessageSort? Sort, bool IsDecending, int? Index, int? Count) : IRequest<ListModel<MessageModel>>;
}
