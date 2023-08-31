using FluentValidation.Results;
using Logitar.EventSourcing;

namespace Logitar.Portal.Application;

public class UniqueNameAlreadyUsedException : Exception
{
  private const string ErrorMessage = "The specified unique name is already used.";

  public UniqueNameAlreadyUsedException(Type type, string? tenantId, string uniqueName, string propertyName)
    : base(BuildMessage(tenantId, uniqueName, propertyName))
  {
    if (!type.IsSubclassOf(typeof(AggregateRoot)))
    {
      throw new ArgumentException($"The type must be a subclass of the '{nameof(AggregateRoot)}' type.", nameof(type));
    }

    TypeName = type.GetName();
    TenantId = tenantId;
    UniqueName = uniqueName;
    PropertyName = propertyName;
  }

  public string TypeName
  {
    get => (string)Data[nameof(TypeName)]!;
    private set => Data[nameof(TypeName)] = value;
  }
  public string? TenantId
  {
    get => (string?)Data[nameof(TenantId)];
    private set => Data[nameof(TenantId)] = value;
  }
  public string UniqueName
  {
    get => (string)Data[nameof(UniqueName)]!;
    private set => Data[nameof(UniqueName)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public ValidationFailure Failure => new(PropertyName, ErrorMessage, UniqueName)
  {
    ErrorCode = "UniqueNameAlreadyUsed",
    CustomState = new { TenantId }
  };

  private static string BuildMessage(string? tenantId, string uniqueName, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append("TenantId: ").AppendLine(tenantId);
    message.Append("UniqueName: ").AppendLine(uniqueName);
    message.Append("PropertyName: ").AppendLine(propertyName);

    return message.ToString();
  }
}

public class UniqueNameAlreadyUsedException<T> : UniqueNameAlreadyUsedException where T : AggregateRoot
{
  public UniqueNameAlreadyUsedException(string? tenantId, string uniqueName, string propertyName)
    : base(typeof(T), tenantId, uniqueName, propertyName)
  {
  }
}
