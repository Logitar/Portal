using GraphQL;
using Logitar.Portal.Application.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.GraphQL;

internal static class ResolveFieldContextExtensions
{
  public static TService GetQueryService<TService, TSource>(this IResolveFieldContext<TSource> context) where TService : class
  {
    context.LogQuery();

    return context.GetRequiredService<TService, TSource>();
  }
  public static TService GetRequiredService<TService, TSource>(this IResolveFieldContext<TSource> context) where TService : notnull
  {
    if (context.RequestServices == null)
    {
      throw new InvalidOperationException($"The {nameof(context.RequestServices)} is required.");
    }

    return context.RequestServices.GetRequiredService<TService>();
  }

  private static void LogQuery<TSource>(this IResolveFieldContext<TSource> context) => LogOperation(context, "query");
  private static void LogOperation<TSource>(this IResolveFieldContext<TSource> context, string type)
  {
    if (context.RequestServices != null)
    {
      ILoggingService loggingService = context.RequestServices.GetRequiredService<ILoggingService>();
      loggingService.SetOperation(new Operation(type, context.FieldDefinition.Name));
    }
  }
}
