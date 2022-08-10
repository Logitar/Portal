using FluentValidation;
using Logitar.Portal.Core;
using Logitar.Portal.Infrastructure;
using Logitar.Portal.Infrastructure.Entities;
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

    public async Task InvokeAsync(HttpContext httpContext, PortalDbContext dbContext, IHostEnvironment environment, IUserContext userContext)
    {
      HttpRequest request = httpContext.Request;
      HttpResponse response = httpContext.Response;

      string? ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
      var log = new Log(request.Method, request.GetDisplayUrl(), ipAddress);

      bool added = false;
      try
      {
        dbContext.Logs.Add(log);
        await dbContext.SaveChangesAsync();
        added = true;
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "{message}", exception.Message);
      }

      try
      {
        await _next(httpContext);
      }
      catch (ApiException exception)
      {
        _logger.LogWarning(exception, "{message}", exception.Message);

        response.StatusCode = (int)exception.StatusCode;

        var data = new Dictionary<string, string?>();
        if (exception.Value != null)
        {
          data.Add(nameof(exception.Value), JsonSerializer.Serialize(exception.Value));

          await response.WriteAsJsonAsync(exception.Value);
        }

        log.Errors.Add(new(exception.StatusCode.ToString(), exception.Message, data));
      }
      catch (ValidationException exception)
      {
        _logger.LogWarning(exception, "{message}", exception.Message);

        response.StatusCode = StatusCodes.Status400BadRequest;

        var data = new Dictionary<string, string?>();
        if (exception.Errors.Any())
        {
          data.Add(nameof(exception.Errors), JsonSerializer.Serialize(exception.Errors));

          await response.WriteAsJsonAsync(exception.Errors);
        }

        log.Errors.Add(new(exception, data));
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "{message}", exception.Message);

        Error error = exception is ErrorException errorException ? errorException.Error : new(exception);
        log.Errors.Add(error);

        response.StatusCode = StatusCodes.Status500InternalServerError;

        if (environment.IsDevelopment())
        {
          await response.WriteAsJsonAsync(error);
        }
      }
      finally
      {
        log.Complete(response.StatusCode, userContext.Actor, httpContext.GetSession(), httpContext.GetUser());

        if (log.HasErrors)
        {
          try
          {
            request.EnableBuffering();
            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body);
            string body = await reader.ReadToEndAsync();

            if (!string.IsNullOrEmpty(body))
            {
              log.Request.Add(nameof(request.Body), body);
            }
          }
          catch (Exception)
          {
          }

          if (request.Headers.Any())
          {
            log.Request.Add(nameof(request.Headers), request.Headers);
          }
        }

        try
        {
          if (added)
          {
            dbContext.Update(log);
          }
          else
          {
            dbContext.Add(log);
          }

          await dbContext.SaveChangesAsync();
        }
        catch (Exception exception)
        {
          _logger.LogError(exception, "{message}", exception.Message);
        }
      }
    }
  }
}
