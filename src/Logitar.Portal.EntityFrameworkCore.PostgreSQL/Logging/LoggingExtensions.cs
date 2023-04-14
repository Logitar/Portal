using Logitar.Portal.Contracts.Errors;
using Microsoft.Extensions.Logging;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Logging;

internal static class LoggingExtensions
{
  public static LogLevel GetLogLevel(this IEnumerable<Error> errors)
  {
    if (errors.Any())
    {
      ErrorSeverity severity = errors.Max(x => x.Severity);
      switch (severity)
      {
        case ErrorSeverity.Critical:
          return LogLevel.Critical;
        case ErrorSeverity.Failure:
          return LogLevel.Error;
        case ErrorSeverity.Warning:
          return LogLevel.Warning;
      }
    }

    return LogLevel.Information;
  }
}
