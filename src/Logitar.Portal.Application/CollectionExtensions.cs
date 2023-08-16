namespace Logitar.Portal.Application;

internal static class CollectionExtensions
{
  public static void AddIfNotNull<T>(this ICollection<T> collection, T? item)
  {
    if (item != null)
    {
      collection.Add(item);
    }
  }
}
