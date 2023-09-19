using Logitar.Portal.Domain.Passwords;

namespace Logitar.Portal.Domain;

internal record PasswordMock : Password
{
  private readonly string _password;

  public PasswordMock(string password)
  {
    _password = password;
  }

  public override string Encode() => _password;

  public override bool IsMatch(string password) => password == _password;
}
