﻿using Logitar.Portal.v2.Contracts.Users;
using Logitar.Portal.v2.Web.Extensions;
using Microsoft.AspNetCore.Authorization;

using CoreConstants = Logitar.Portal.v2.Core.Constants;

namespace Logitar.Portal.v2.Web.Authorization;

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
      User? user = _httpContextAccessor.HttpContext.GetUser();
      if (user != null)
      {
        if (user.Realm.UniqueName == CoreConstants.PortalRealm.UniqueName)
        {
          context.Succeed(requirement);
        }
        else
        {
          context.Fail(new AuthorizationFailureReason(this, "The User should not belong to a Realm."));
        }
      }
      //else if (_httpContextAccessor.HttpContext.GetApiKey() != null)
      //{
      //  context.Succeed(requirement);
      //} // TODO(fpion): ApiKey
    }

    return Task.CompletedTask;
  }
}
