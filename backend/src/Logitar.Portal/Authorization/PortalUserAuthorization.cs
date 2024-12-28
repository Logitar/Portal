using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Web.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Logitar.Portal.Authorization;

internal class PortalUserAuthorizationRequirement : IAuthorizationRequirement;

internal class PortalUserAuthorizationHandler : AuthorizationHandler<PortalUserAuthorizationRequirement>
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public PortalUserAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PortalUserAuthorizationRequirement requirement)
  {
    HttpContext? httpContext = _httpContextAccessor.HttpContext;
    if (httpContext != null)
    {
      UserModel? user = httpContext.GetUser();
      if (user != null)
      {
        if (user.Realm == null)
        {
          context.Succeed(requirement);
        }
        else
        {
          context.Fail(new AuthorizationFailureReason(this, "The User should not belong to a Realm."));
        }
      }
    }

    return Task.CompletedTask;
  }
}
