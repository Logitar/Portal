namespace Logitar.Portal.Web.Models.Users
{
  internal class CurrentUser
  {
    /// <summary>
    /// TODO(fpion): implement
    /// </summary>
    public CurrentUser(/*User? user*/)
    {
      //IsAuthenticated = user != null;

      //Email = user?.Email;
      //FullName = user?.FullName;
      //Picture = user?.Picture;
      //Username = user?.Username;
    }

    public bool IsAuthenticated { get; }

    public string? Email { get; }
    public string? FullName { get; }
    public string? Picture { get; }
    public string? Username { get; }
  }
}
