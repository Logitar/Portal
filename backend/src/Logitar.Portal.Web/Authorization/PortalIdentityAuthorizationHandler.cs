using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Web.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Logitar.Portal.Web.Authorization
{
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
        UserModel? user = _httpContextAccessor.HttpContext.GetUser();
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
