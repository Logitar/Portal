using Logitar.Portal.Application;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Infrastructure;
using Logitar.Portal.Web.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using System.Text.Json;

namespace Logitar.Portal.Web.Middlewares
{
  internal class Logging
  {
    private readonly ILogger<Logging> _logger;
    private readonly RequestDelegate _next;

    public Logging(ILogger<Logging> logger, RequestDelegate next)
    {
      _logger = logger;
      _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ILoggingContext logging, IUserContext userContext)
    {
      HttpRequest request = context.Request;
      HttpResponse response = context.Response;

      try
      {
        try
        {
          string? ipAddress = context.Connection.RemoteIpAddress?.ToString();
          string? additionalInformation = JsonSerializer.Serialize(request.Headers);
          logging.Start(context.TraceIdentifier, request.Method, request.GetDisplayUrl(), ipAddress, additionalInformation);
        }
        catch (Exception exception)
        {
          _logger.LogError(exception, "An error occurred while starting the current log.");
        }

        await _next(context);
      }
      catch (Exception exception)
      {
        try
        {
          logging.AddError(exception);
        }
        catch (Exception addErrorException)
        {
          _logger.LogError(addErrorException, "An error occurred while adding an error to the current log.");
        }

        throw;
      }
      finally
      {
        try
        {
          ActorModel actor = userContext.Actor;
          ApiKeyModel? apiKey = context.GetApiKey();
          UserModel? user = context.GetUser();
          SessionModel? session = context.GetSession();
          await logging.CompleteAsync(response.StatusCode, actor, apiKey, user, session);
        }
        catch (Exception exception)
        {
          _logger.LogError(exception, "An error occurred while completing the current log.");
        }
      }
    }
  }
}
