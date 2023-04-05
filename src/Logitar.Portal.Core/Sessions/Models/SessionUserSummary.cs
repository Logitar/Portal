namespace Logitar.Portal.Core.Sessions.Models
{
  public class SessionUserSummary
  {
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string? Email { get; set; }

    public string? FullName { get; set; }
    public string? Picture { get; set; }
  }
}
