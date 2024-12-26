using Logitar.Identity.Contracts.Users;

namespace Logitar.Portal.Contracts.Users;

public record EmailModel : ContactModel, IEmail
{
  public string Address { get; set; }

  public EmailModel() : this(string.Empty)
  {
  }

  public EmailModel(string address)
  {
    Address = address;
  }
}
