using Logitar.Portal.v2.Contracts.Users.Contact;

namespace Logitar.Portal.v2.Core.Users.Contact;

public record ReadOnlyEmail : ReadOnlyContact
{
  public ReadOnlyEmail(EmailInput input) : base(input.Verify)
  {
    Address = input.Address.Trim();
  }

  public string Address { get; }
}
