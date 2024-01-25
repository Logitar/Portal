using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Extensions;

internal static class HttpContextExtensions
{
  private const string RealmKey = nameof(Realm);

  public static Realm? GetRealm(this HttpContext context) => context.GetItem<Realm>(RealmKey);
  private static T? GetItem<T>(this HttpContext context, object key) => context.Items.TryGetValue(key, out object? value) ? (T?)value : default;

  public static void SetRealm(this HttpContext context, Realm? realm) => context.SetItem(RealmKey, realm);
  private static void SetItem(this HttpContext context, object key, object? value)
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
