namespace Portal.Web.Models.Users
{
  internal class UserSummary
  {
    public bool IsAuthenticated { get; set; }

    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? Picture { get; set; }
    public string? Username { get; set; }
  }
}
