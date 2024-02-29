using Logitar.Portal.Application.Users;
using Logitar.Portal.Constants;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Extensions;
using Logitar.Portal.Web.Extensions;
using Microsoft.Extensions.Primitives;

namespace Logitar.Portal.Middlewares;

internal class ResolveUser
{
  private readonly RequestDelegate _next;

  public ResolveUser(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context, IUserService userService)
  {
    if (context.Request.Headers.TryGetValue(Headers.User, out StringValues headerValues))
    {
      IReadOnlyCollection<string> values = headerValues.ExtractValues();
      if (values.Count > 0)
      {
        if (context.GetApiKey() == null)
        {
          context.Response.Headers.WWWAuthenticate = Schemes.ApiKey;
          Error error = new("ApiKeyIsRequired", "An API key is required to act on behalf of an user.");
          await context.SetResponseAsync(StatusCodes.Status401Unauthorized, error);
          return;
        }
        else if (values.Count > 1)
        {
          Error error = new("TooManyUsers", $"The '{Headers.User}' header only supports one value.");
          await context.SetResponseAsync(StatusCodes.Status400BadRequest, error);
          return;
        }

        string value = values.Single();
        bool parsed = Guid.TryParse(value, out Guid id);
        User? user = await userService.ReadAsync(id: parsed ? id : null, uniqueName: value);
        if (user == null)
        {
          Error error = new("UserNotFound", $"The user '{value}' could not be found.");
          await context.SetResponseAsync(StatusCodes.Status404NotFound, error);
          return;
        }

        context.SetUser(user);
      }
    }

    await _next(context);
  }
}
