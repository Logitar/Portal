using Logitar.Portal.Domain;
using System.Text;

namespace Logitar.Portal.Application
{
  public class EntityNotFoundException : Exception
  {
    protected EntityNotFoundException(Type type, string id, string? paramName)
      : base(GetMessage(type, id, paramName))
    {
      Data["Type"] = type.GetName();
      Data["Id"] = id;

      if (paramName != null)
      {
        Data["ParamName"] = paramName;
      }
    }

    private static string GetMessage(Type type, string id, string? paramName)
    {
      StringBuilder message = new();

      message.AppendLine("The specified entity could not be found.");
      message.AppendLine($"Type: {type}");
      message.AppendLine($"Id: {id}");

      if (paramName != null)
      {
        message.AppendLine($"ParamName: {paramName}");
      }

      return message.ToString();
    }
  }

  internal class EntityNotFoundException<T> : EntityNotFoundException
  {
    public EntityNotFoundException(string id, string? paramName = null)
      : base(typeof(T), id, paramName)
    {
    }
  }
}
