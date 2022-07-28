using Microsoft.AspNetCore.Authorization;
using Portal.Core.Users;

namespace Portal.Web.Authorization
{
  internal class PortalIdentityAuthorizationRequirement : IAuthorizationRequirement
  {
  }

  internal class PortalIdentityAuthorizationHandler : AuthorizationHandler<PortalIdentityAuthorizationRequirement>
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PortalIdentityAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PortalIdentityAuthorizationRequirement requirement)
    {
      if (_httpContextAccessor.HttpContext != null)
      {
        User? user = _httpContextAccessor.HttpContext.GetUser();
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
        else if (_httpContextAccessor.HttpContext.GetApiKey() != null)
        {
          context.Succeed(requirement);
        }
      }

      return Task.CompletedTask;
    }
  }
}
