namespace Logitar.Portal.Contracts.Users;

public record Email : Contact, IEmailAddress
{
  public Email() : this(string.Empty)
  {
  }
  public Email(string address) : base()
  {
    Address = address;
  }

  public string Address { get; set; }
}
