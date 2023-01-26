namespace Logitar.Portal.Core2
{
  public static class TypeExtensions
  {
    public static string GetName(this Type type) => type.AssemblyQualifiedName ?? type.FullName ?? type.Name;
  }
}
