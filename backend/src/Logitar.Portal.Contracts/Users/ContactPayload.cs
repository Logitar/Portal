namespace Logitar.Portal.Contracts.Users;

public abstract record ContactPayload
{
  public bool IsVerified { get; set; }

  public ContactPayload(bool isVerified = false)
  {
    IsVerified = isVerified;
  }
}
