using System.Text;

namespace Logitar.Portal.Core
{
  internal class EntityNotFoundException : Exception
  {
    private EntityNotFoundException(Type type, string id, string? paramName)
      : base(GetMessage(type, id, paramName))
    {
      Data["Type"] = type.GetName();
      Data["Id"] = id;

      if (paramName != null)
      {
        Data["ParamName"] = paramName;
      }
    }

    public static EntityNotFoundException Typed<T>(string id, string? paramName = null) => new(typeof(T), id, paramName);

    private static string GetMessage(Type type, string id, string? paramName)
    {
      StringBuilder message = new();

      message.AppendLine("The specified entity could not be found.");
      message.AppendLine($"Type: {type.GetName()}");
      message.AppendLine($"Id: {id}");

      if (paramName != null)
      {
        message.AppendLine($"ParamName: {paramName}");
      }

      return message.ToString();
    }
  }
}
