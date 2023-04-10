﻿using Logitar.Portal.v2.Contracts.Sessions;
using Logitar.Portal.v2.Web.Extensions;
using MediatR;
using CoreConstants = Logitar.Portal.v2.Core.Constants;

namespace Logitar.Portal.v2.Web.Commands;

internal class PortalSignInHandler : IRequestHandler<PortalSignIn, Session>
{
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly ISessionService _sessionService;

  public PortalSignInHandler(IHttpContextAccessor httpContextAccessor, ISessionService sessionService)
  {
    _httpContextAccessor = httpContextAccessor;
    _sessionService = sessionService;
  }

  public async Task<Session> Handle(PortalSignIn request, CancellationToken cancellationToken)
  {
    HttpContext? context = _httpContextAccessor.HttpContext;

    SignInInput input = new()
    {
      Realm = CoreConstants.PortalRealm.UniqueName,
      Username = request.Username,
      Password = request.Password,
      Remember = request.Remember,
      IpAddress = context?.GetIpAddress(),
      AdditionalInformation = context?.GetAdditionalInformation()
    };

    return await _sessionService.SignInAsync(input, cancellationToken);
  }
}
