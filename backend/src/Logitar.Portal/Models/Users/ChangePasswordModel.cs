using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Models.Users;

public record ChangePasswordModel
{
  public string Current { get; set; }
  public string New { get; set; }

  public ChangePasswordModel() : this(string.Empty, string.Empty)
  {
  }

  public ChangePasswordModel(string currentPassword, string newPassword)
  {
    Current = currentPassword;
    New = newPassword;
  }

  public ChangePasswordPayload ToPayload() => new(New)
  {
    Current = Current
  };
}
