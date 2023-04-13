using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Web.Extensions;
using Microsoft.AspNetCore.Authorization;

using CoreConstants = Logitar.Portal.Core.Constants;

namespace Logitar.Portal.Web.Authorization;

internal class AuthenticatedPortalUserAuthorizationHandler : AuthorizationHandler<AuthenticatedPortalUserAuthorizationRequirement>
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public AuthenticatedPortalUserAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthenticatedPortalUserAuthorizationRequirement requirement)
  {
    if (_httpContextAccessor.HttpContext != null)
    {
      User? user = _httpContextAccessor.HttpContext.GetUser();
      if (user == null)
      {
        context.Fail(new AuthorizationFailureReason(this, "The User is required."));
      }
      else if (user.Realm.UniqueName != CoreConstants.PortalRealm.UniqueName)
      {
        context.Fail(new AuthorizationFailureReason(this, "The User should not belong to a Realm."));
      }
      else if (_httpContextAccessor.HttpContext.GetSession() == null)
      {
        context.Fail(new AuthorizationFailureReason(this, "The Session is required."));
      }
      else
      {
        context.Succeed(requirement);
      }
    }

    return Task.CompletedTask;
  }
}
