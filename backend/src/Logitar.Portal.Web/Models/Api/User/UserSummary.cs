namespace Logitar.Portal.Web.Models.Api.User
{
  public class UserSummary
  {
    public Guid Id { get; set; }
    public DateTime UpdatedAt { get; set; }

    public string Username { get; set; } = null!;

    public string? Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsPhoneNumberConfirmed { get; set; }

    public string? FullName { get; set; }
    public string? Picture { get; set; }

    public DateTime? PasswordChangedAt { get; set; }
    public DateTime? SignedInAt { get; set; }

    public bool IsDisabled { get; set; }
  }
}
