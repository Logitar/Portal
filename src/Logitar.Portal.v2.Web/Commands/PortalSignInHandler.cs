using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Sessions;
using MediatR;
using System.Text.Json;

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
    List<CustomAttribute> customAttributes = new(capacity: 2);
    if (_httpContextAccessor.HttpContext != null)
    {
      customAttributes.Add(new CustomAttribute
      {
        Key = "RequestHeaders",
        Value = JsonSerializer.Serialize(_httpContextAccessor.HttpContext.Request.Headers)
      });

      if (_httpContextAccessor.HttpContext.Connection.RemoteIpAddress != null)
      {
        customAttributes.Add(new CustomAttribute
        {
          Key = "RemoteIpAddress", // TODO(fpion): CLIENT-IP header?
          Value = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString()
        });
      }
    }

    SignInInput input = new()
    {
      Realm = CoreConstants.PortalRealm.UniqueName,
      Username = request.Username,
      Password = request.Password,
      Remember = request.Remember,
      CustomAttributes = customAttributes
    };

    return await _sessionService.SignInAsync(input, cancellationToken);
  }
}
