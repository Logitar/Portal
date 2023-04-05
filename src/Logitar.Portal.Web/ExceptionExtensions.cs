using System.Reflection;
using System.Text.Json;

namespace Logitar.Portal.Web
{
  internal static class ExceptionExtensions
  {
    public static IReadOnlyDictionary<string, string?> GetData(this Exception exception)
    {
      ArgumentNullException.ThrowIfNull(exception);

      PropertyInfo[] properties = exception.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
      var data = new Dictionary<string, string?>(capacity: properties.Length);

      foreach (PropertyInfo property in properties)
      {
        try
        {
          object? value = property.GetValue(exception);
          if (value != null)
          {
            string json = JsonSerializer.Serialize(value);
            if (!string.IsNullOrEmpty(json))
            {
              data.Add(property.Name, json);
            }
          }
        }
        catch (Exception)
        {
        }
      }

      return data;
    }
  }
}
