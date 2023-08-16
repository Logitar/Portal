using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Extensions;

internal static class HttpContextExtensions
{
  private const string ConfigurationKey = "Configuration";
  private const string RealmKey = "Realm";

  public static ConfigurationAggregate? GetConfiguration(this HttpContext context)
    => context.GetItem<ConfigurationAggregate>(ConfigurationKey);
  public static RealmAggregate? GetRealm(this HttpContext context)
    => context.GetItem<RealmAggregate>(RealmKey);
  private static T? GetItem<T>(this HttpContext context, object key)
    => context.Items.TryGetValue(key, out object? value) ? (T?)value : default;

  public static void SetConfiguration(this HttpContext context, ConfigurationAggregate? configuration)
    => context.SetItem(ConfigurationKey, configuration);
  public static void SetRealm(this HttpContext context, RealmAggregate? realm)
    => context.SetItem(RealmKey, realm);
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
