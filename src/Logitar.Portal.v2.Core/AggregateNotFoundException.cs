using Logitar.EventSourcing;
using System.Text;

namespace Logitar.Portal.v2.Core;

internal class AggregateNotFoundException : Exception
{
  public AggregateNotFoundException(Type type, string id) : base(GetMessage(type, id))
  {
    Data["Type"] = type.GetName();
    Data["Id"] = id;
  }

  private static string GetMessage(Type type, string id)
  {
    StringBuilder message = new();

    message.AppendLine("The specified aggregate could not be found.");
    message.Append("Type: ").Append(type.GetName()).AppendLine();
    message.Append("Id: ").Append(id).AppendLine();

    return message.ToString();
  }
}

internal class AggregateNotFoundException<T> : AggregateNotFoundException
{
  public AggregateNotFoundException(Guid id) : base(typeof(T), id.ToString())
  {
  }
}
