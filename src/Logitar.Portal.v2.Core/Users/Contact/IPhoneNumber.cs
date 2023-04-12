namespace Logitar.Portal.v2.Core.Users.Contact;

public interface IPhoneNumber
{
  string? CountryCode { get; }
  string Number { get; }
  string? Extension { get; }
}
