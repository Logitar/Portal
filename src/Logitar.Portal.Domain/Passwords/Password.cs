namespace Logitar.Portal.Domain.Passwords;

public abstract record Password
{
  public abstract string Encode();
}
