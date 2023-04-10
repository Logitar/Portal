﻿using Logitar.Portal.v2.Core;

namespace Logitar.Portal.v2.Web;

public class HttpApplicationContext : IApplicationContext
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public HttpApplicationContext(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public Uri? BaseUrl
  {
    get
    {
      if (_httpContextAccessor.HttpContext == null)
      {
        return null;
      }

      HttpRequest request = _httpContextAccessor.HttpContext.Request;

      return new Uri($"{request.Scheme}://{request.Host}");
    }
  }
}
