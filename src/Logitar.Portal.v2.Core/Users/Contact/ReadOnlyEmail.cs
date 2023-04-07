using Logitar.Portal.v2.Contracts.Users.Contact;

namespace Logitar.Portal.v2.Core.Users.Contact;

public record ReadOnlyEmail : ReadOnlyContact
{
  public ReadOnlyEmail(EmailInput input) : this(input.Address, input.Verify)
  {
  }
  public ReadOnlyEmail(string address, bool isVerified = false) : base(isVerified)
  {
    Address = address.Trim();
  }

  public string Address { get; }

  public ReadOnlyEmail AsVerified() => new(Address, isVerified: true);
}
