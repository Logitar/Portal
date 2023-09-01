using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Web.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Logitar.Portal.Web.Authorization;

internal class PortalActorAuthorizationHandler : AuthorizationHandler<PortalActorAuthorizationRequirement>
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public PortalActorAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PortalActorAuthorizationRequirement requirement)
  {
    if (_httpContextAccessor.HttpContext != null)
    {
      // TODO(fpion): add a claim for the realm and use it

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
      else
      {
        ApiKey? apiKey = _httpContextAccessor.HttpContext.GetApiKey();
        if (apiKey != null)
        {
          if (apiKey.Realm == null)
          {
            context.Succeed(requirement);
          }
          else
          {
            context.Fail(new AuthorizationFailureReason(this, "The API key should not belong to a Realm."));
          }
        }
      }
    }

    return Task.CompletedTask;
  }
}
