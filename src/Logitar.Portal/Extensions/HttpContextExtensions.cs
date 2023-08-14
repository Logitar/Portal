using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Extensions;

internal static class HttpContextExtensions
{
  private const string ConfigurationKey = "Configuration";

  public static ConfigurationAggregate? GetConfiguration(this HttpContext context)
    => context.GetItem<ConfigurationAggregate>(ConfigurationKey);
  private static T? GetItem<T>(this HttpContext context, object key)
    => context.Items.TryGetValue(key, out object? value) ? (T?)value : default;

  public static void SetConfiguration(this HttpContext context, ConfigurationAggregate? configuration)
    => context.SetItem(ConfigurationKey, configuration);
  private static void SetItem<T>(this HttpContext context, object key, T? value)
  {
    if (value == null)
    {
      context.Items.Remove(key);
    }
    else
    {
      context.Items[key] = value;
    }
  }
}
