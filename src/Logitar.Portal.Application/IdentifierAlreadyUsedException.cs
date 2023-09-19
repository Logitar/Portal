using FluentValidation.Results;
using Logitar.EventSourcing;
using Logitar.Portal.Domain;

namespace Logitar.Portal.Application;

public class IdentifierAlreadyUsedException : Exception, IFailureException
{
  private const string ErrorMessage = "The specified identifier is already used.";

  public IdentifierAlreadyUsedException(Type type, string? tenantId, string key, string value, string propertyName)
    : base(BuildMessage(type, tenantId, key, value, propertyName))
  {
    if (!type.IsSubclassOf(typeof(AggregateRoot)))
    {
      throw new ArgumentException($"The type must be a subclass of the '{nameof(AggregateRoot)}' type.", nameof(type));
    }

    TypeName = type.GetName();
    TenantId = tenantId;
    Key = key;
    Value = value;
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
  public string Key
  {
    get => (string)Data[nameof(Key)]!;
    private set => Data[nameof(Key)] = value;
  }
  public string Value
  {
    get => (string)Data[nameof(Value)]!;
    private set => Data[nameof(Value)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public ValidationFailure Failure => new(PropertyName, ErrorMessage, attemptedValue: new { Key, Value })
  {
    ErrorCode = "IdentifierAlreadyUsed"
  };

  private static string BuildMessage(Type type, string? tenantId, string key, string value, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append("TypeName: ").AppendLine(type.GetName());
    message.Append("TenantId: ").AppendLine(tenantId ?? "<null>");
    message.Append("Key: ").AppendLine(key);
    message.Append("Value: ").AppendLine(value);
    message.Append("PropertyName: ").AppendLine(propertyName);

    return message.ToString();
  }
}

public class IdentifierAlreadyUsedException<T> : IdentifierAlreadyUsedException where T : AggregateRoot
{
  public IdentifierAlreadyUsedException(string? tenantId, string key, string value, string propertyName)
    : base(typeof(T), tenantId, key, value, propertyName)
  {
  }
}
