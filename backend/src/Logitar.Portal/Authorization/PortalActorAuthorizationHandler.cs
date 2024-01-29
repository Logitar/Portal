﻿using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Logitar.Portal.Authorization;

internal class PortalActorAuthorizationHandler : AuthorizationHandler<PortalActorAuthorizationRequirement>
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public PortalActorAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PortalActorAuthorizationRequirement requirement)
  {
    HttpContext? httpContext = _httpContextAccessor.HttpContext;
    if (httpContext != null)
    {
      User? user = httpContext.GetUser();
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
        ApiKey? apiKey = httpContext.GetApiKey();
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
