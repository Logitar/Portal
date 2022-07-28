namespace Portal.Core
{
  public static class TypeExtensions
  {
    public static string GetName(this Type type)
    {
      ArgumentNullException.ThrowIfNull(type);

      return type.AssemblyQualifiedName ?? type.FullName ?? type.Name;
    }
  }
}
