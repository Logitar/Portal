using Microsoft.AspNetCore.Authorization;
using Portal.Core.Users;

namespace Portal.Web.Authorization
{
  internal class UserAuthorizationRequirement : IAuthorizationRequirement
  {
  }

  internal class UserAuthorizationHandler : AuthorizationHandler<UserAuthorizationRequirement>
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserAuthorizationRequirement requirement)
    {
      if (_httpContextAccessor.HttpContext != null)
      {
        User? user = _httpContextAccessor.HttpContext.GetUser();
        if (user == null)
        {
          context.Fail(new AuthorizationFailureReason(this, "The User is required."));
        }
        else
        {
          context.Succeed(requirement);
        }
      }

      return Task.CompletedTask;
    }
  }
}
