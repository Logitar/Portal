namespace Portal.Web.Models.Users
{
  internal class UserSummary
  {
    public bool IsAuthenticated { get; set; }

    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Picture { get; set; }
  }
}
