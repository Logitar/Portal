using Logitar.EventSourcing;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using System.Text;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

public class EntityNotFoundException : Exception
{
  private const string ErrorMessage = "The specified entity could not be found.";

  public EntityNotFoundException(Type type, AggregateId aggregateId)
    : base(BuildMessage(type, aggregateId))
  {
    if (!type.IsSubclassOf(typeof(AggregateEntity)))
    {
      throw new ArgumentException($"The type must be a subclass of the '{nameof(AggregateEntity)}' type.", nameof(type));
    }

    TypeName = type.GetName();
    AggregateId = aggregateId.Value;
  }

  public string TypeName
  {
    get => (string)Data[nameof(TypeName)]!;
    private set => Data[nameof(TypeName)] = value;
  }
  public string AggregateId
  {
    get => (string)Data[nameof(AggregateId)]!;
    private set => Data[nameof(AggregateId)] = value;
  }

  private static string BuildMessage(Type type, AggregateId aggregateId)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append("TypeName: ").AppendLine(type.GetName());
    message.Append("AggregateId: ").Append(aggregateId).AppendLine();

    return message.ToString();
  }
}

internal class EntityNotFoundException<T> : EntityNotFoundException where T : AggregateEntity
{
  public EntityNotFoundException(AggregateId aggregateId) : base(typeof(T), aggregateId)
  {
  }
}
