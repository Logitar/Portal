using Microsoft.AspNetCore.Authorization;

namespace Logitar.Portal.Web.Authorization
{
  internal class SessionAuthorizationRequirement : IAuthorizationRequirement
  {
  }

  internal class SessionAuthorizationHandler : AuthorizationHandler<SessionAuthorizationRequirement>
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SessionAuthorizationRequirement requirement)
    {
      if (_httpContextAccessor.HttpContext != null)
      {
        if (_httpContextAccessor.HttpContext.GetSession() == null)
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
}
