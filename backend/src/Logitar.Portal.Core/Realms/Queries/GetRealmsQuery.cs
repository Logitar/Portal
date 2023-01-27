using Logitar.Portal.Core.Models;
using Logitar.Portal.Core.Realms.Models;
using MediatR;

namespace Logitar.Portal.Core.Realms.Queries
{
  internal class GetRealmsQuery : IRequest<ListModel<RealmModel>>
  {
    public string? Search { get; set; }

    public RealmSort? Sort { get; set; }
    public bool IsDescending { get; set; }

    public int? Index { get; set; }
    public int? Count { get; set; }
  }
}
