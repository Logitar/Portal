using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Users.Contact;

namespace Logitar.Portal.v2.Core.Users.Contact;

public record ReadOnlyPhone : ReadOnlyContact, IPhoneNumber
{
  public ReadOnlyPhone(PhoneInput input) : base(input.Verify)
  {
    CountryCode = input.CountryCode?.CleanTrim();
    Number = input.Number.Trim();
    Extension = input.Extension?.CleanTrim();
  }

  public string? CountryCode { get; }
  public string Number { get; }
  public string? Extension { get; }
}
