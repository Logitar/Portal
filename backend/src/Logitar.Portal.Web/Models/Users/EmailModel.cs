using Logitar.Identity.Contracts.Users;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Web.Models.Users;

public record EmailModel : IEmail
{
  public string Address { get; set; }

  public EmailModel() : this(string.Empty)
  {
  }

  public EmailModel(string address)
  {
    Address = address;
  }

  public EmailPayload ToPayload() => new(Address, isVerified: false);
}
