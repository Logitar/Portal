namespace Logitar.Portal.v2.Web.Models;

/// <summary>
/// TODO(fpion): implement
/// </summary>
internal record CurrentUser
{
  public CurrentUser(/*User? user*/)
  {
    //IsAuthenticated = user != null;

    //Email = user?.Email;
    //FullName = user?.FullName;
    //Picture = user?.Picture;
    //Username = user?.Username;
  }

  public bool IsAuthenticated { get; }

  public string? EmailAddress { get; }
  public string? FullName { get; }
  public string? Picture { get; }
  public string? Username { get; }
}
