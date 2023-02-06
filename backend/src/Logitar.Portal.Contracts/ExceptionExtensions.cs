using System.Text.Json;

namespace Logitar.Portal.Contracts
{
  public static class ExceptionExtensions
  {
    public static string GetCode(this Exception exception) => exception.GetType().Name.Replace(nameof(Exception), string.Empty);

    public static Dictionary<string, string?>? GetData(this Exception exception)
    {
      if (exception.Data.Count == 0)
      {
        return null;
      }

      Dictionary<string, string?> data = new(capacity: exception.Data.Count);
      foreach (object key in exception.Data.Keys)
      {
        data[key is string s ? s : (string)key] = JsonSerializer.Serialize(exception.Data[key]);
      }

      return data;
    }
  }
}
