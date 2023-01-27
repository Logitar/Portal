using Logitar.Portal.Core.Models;
using Logitar.Portal.Core.Sessions.Models;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Queries
{
  internal class GetSessionsQuery : IRequest<ListModel<SessionModel>>
  {
    public bool? IsActive { get; set; }
    public bool? IsPersistent { get; set; }
    public string? Realm { get; set; }
    public string? UserId { get; set; }

    public SessionSort? Sort { get; set; }
    public bool IsDescending { get; set; }

    public int? Index { get; set; }
    public int? Count { get; set; }
  }
}
