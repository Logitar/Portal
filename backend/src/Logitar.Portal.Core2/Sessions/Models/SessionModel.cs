using Logitar.Portal.Core2.Models;

namespace Logitar.Portal.Core2.Sessions.Models
{
  public class SessionModel : AggregateModel
  {
    public string? RenewToken { get; set; }
  }
}
