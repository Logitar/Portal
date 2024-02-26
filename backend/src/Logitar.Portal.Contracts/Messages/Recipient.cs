using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Contracts.Messages;

public record Recipient
{
  public RecipientType Type { get; set; }

  public string Address { get; set; }
  public string? DisplayName { get; set; }

  public User? User { get; set; }

  public Recipient() : this(string.Empty)
  {
  }

  public Recipient(string address)
  {
    Address = address;
  }
}
