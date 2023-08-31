using Logitar.EventSourcing;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

public class EntityNotFoundException : Exception
{
  private const string ErrorMessage = "The specified entity could not be found.";

  public EntityNotFoundException(Type type, string aggregateId)
    : base(BuildMessage(type, aggregateId))
  {
    if (!type.IsSubclassOf(typeof(AggregateEntity)))
    {
      throw new ArgumentException($"The type must be a subclass of the '{nameof(AggregateEntity)}' type.", nameof(type));
    }

    TypeName = type.GetName();
    AggregateId = aggregateId;
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

  private static string BuildMessage(Type type, string aggregateId)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append("TypeName: ").AppendLine(type.GetName());
    message.Append("AggregateId: ").AppendLine(aggregateId);

    return message.ToString();
  }
}

internal class EntityNotFoundException<T> : EntityNotFoundException where T : AggregateEntity
{
  public EntityNotFoundException(AggregateId aggregateId) : this(aggregateId.Value)
  {
  }
  public EntityNotFoundException(string aggregateId) : base(typeof(T), aggregateId)
  {
  }
}
