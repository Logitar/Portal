using Logitar.Portal.Core.Models;

namespace Logitar.Portal.Core.Sessions.Models
{
  public class SessionModel : AggregateModel
  {
    public string? RenewToken { get; set; }
  }
}
