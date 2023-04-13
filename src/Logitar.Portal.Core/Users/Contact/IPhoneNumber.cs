namespace Logitar.Portal.Core.Users.Contact;

public interface IPhoneNumber
{
  string? CountryCode { get; }
  string Number { get; }
  string? Extension { get; }
}
