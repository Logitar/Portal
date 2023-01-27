using Logitar.Portal.Core.Models;
using Logitar.Portal.Core.Users.Models;
using MediatR;

namespace Logitar.Portal.Core.Users.Queries
{
  internal class GetUsersQuery : IRequest<ListModel<UserModel>>
  {
    public bool? IsConfirmed { get; set; }
    public bool? IsDisabled { get; set; }
    public string? Realm { get; set; }
    public string? Search { get; set; }

    public UserSort? Sort { get; set; }
    public bool IsDescending { get; set; }

    public int? Index { get; set; }
    public int? Count { get; set; }
  }
}
