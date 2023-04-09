using Logitar.Portal.v2.Contracts.Users.Contact;

namespace Logitar.Portal.v2.Core.Users.Contact;

public record ReadOnlyEmail : ReadOnlyContact
{
  public ReadOnlyEmail(string address, bool isVerified = false) : base(isVerified)
  {
    Address = address.Trim();
  }

  public string Address { get; }

  public static ReadOnlyEmail? From(EmailInput? input)
  {
    return input == null ? null : new(input.Address, input.Verify);
  }

  public ReadOnlyEmail AsVerified() => new(Address, isVerified: true);
}
