namespace Logitar.Portal.Domain.Passwords;

public interface IPasswordService
{
  Password Create(string password);
  Password Generate(int length, out byte[] password);
}
