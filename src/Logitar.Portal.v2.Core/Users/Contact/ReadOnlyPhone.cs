using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Users.Contact;

namespace Logitar.Portal.v2.Core.Users.Contact;

public record ReadOnlyPhone : ReadOnlyContact, IPhoneNumber
{
  public ReadOnlyPhone(string number, string? countryCode = null, string? extension = null,
    bool isVerified = false) : base(isVerified)
  {
    CountryCode = countryCode?.CleanTrim();
    Number = number.Trim();
    Extension = extension?.CleanTrim();
  }

  public string? CountryCode { get; }
  public string Number { get; }
  public string? Extension { get; }

  public static ReadOnlyPhone? From(PhoneInput? input)
  {
    return input == null ? null : new(input.Number, input.CountryCode, input.Extension, input.Verify);
  }

  public ReadOnlyPhone AsVerified() => new(Number, CountryCode, Extension, isVerified: true);
}
