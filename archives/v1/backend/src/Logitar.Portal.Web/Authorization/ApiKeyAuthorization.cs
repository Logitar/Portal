using Microsoft.AspNetCore.Authorization;

namespace Logitar.Portal.Web.Authorization
{
  internal class ApiKeyAuthorizationRequirement : IAuthorizationRequirement
  {
  }

  internal class ApiKeyAuthorizationHandler : AuthorizationHandler<ApiKeyAuthorizationRequirement>
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiKeyAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyAuthorizationRequirement requirement)
    {
      if (_httpContextAccessor.HttpContext != null)
      {
        if (_httpContextAccessor.HttpContext.GetApiKey() == null)
        {
          context.Fail(new AuthorizationFailureReason(this, "The API key is required."));
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
