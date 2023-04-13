using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Web.Models;

internal record CurrentUser
{
  public CurrentUser(User? user)
  {
    IsAuthenticated = user != null;

    EmailAddress = user?.Email?.Address;
    FullName = user?.FullName;
    Picture = user?.Picture;
    Username = user?.Username;
  }

  public bool IsAuthenticated { get; }

  public string? EmailAddress { get; }
  public string? FullName { get; }
  public string? Picture { get; }
  public string? Username { get; }
}
