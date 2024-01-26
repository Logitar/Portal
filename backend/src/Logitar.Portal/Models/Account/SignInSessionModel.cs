using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Models.Account;

public record SignInSessionModel
{
  public string UniqueName { get; set; }
  public string Password { get; set; }
  public bool IsPersistent { get; set; }

  public SignInSessionModel() : this(string.Empty, string.Empty)
  {
  }

  public SignInSessionModel(string uniqueName, string password, bool isPersistent = false)
  {
    UniqueName = uniqueName;
    Password = password;
    IsPersistent = isPersistent;
  }

  public SignInSessionPayload ToPayload(IEnumerable<CustomAttribute> customAttributes)
  {
    SignInSessionPayload payload = new(UniqueName, Password, IsPersistent);
    payload.CustomAttributes.AddRange(customAttributes);

    return payload;
  }
}
