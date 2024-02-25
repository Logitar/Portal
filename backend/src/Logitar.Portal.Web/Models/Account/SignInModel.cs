using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Web.Models.Account;

public record SignInModel
{
  public string UniqueName { get; set; }
  public string Password { get; set; }
  public bool IsPersistent { get; set; }

  public SignInModel() : this(string.Empty, string.Empty)
  {
  }

  public SignInModel(string uniqueName, string password)
  {
    UniqueName = uniqueName;
    Password = password;
  }

  public SignInSessionPayload ToPayload(IEnumerable<CustomAttribute> customAttributes)
  {
    return new SignInSessionPayload(UniqueName, Password, IsPersistent, customAttributes);
  }
}
