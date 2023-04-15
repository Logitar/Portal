using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Core.Configurations;
using Logitar.Portal.Web.Models;
using MediatR;

namespace Logitar.Portal.Web.Commands;

internal record PortalSignIn : IRequest<Session>
{
  public PortalSignIn(InitializeConfigurationInput input)
  {
    Username = input.User.Username;
    Password = input.User.Password;
  }
  public PortalSignIn(PortalSignInInput input)
  {
    Username = input.Username;
    Password = input.Password;
    Remember = input.Remember;
  }

  public string Username { get; }
  public string Password { get; }
  public bool Remember { get; }
}
