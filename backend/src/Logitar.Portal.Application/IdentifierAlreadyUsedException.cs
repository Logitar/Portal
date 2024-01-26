using Logitar.EventSourcing;

namespace Logitar.Portal.Application;

public class IdentifierAlreadyUsedException : Exception
{
  public const string ErrorMessage = "The specified identifier is already used.";

  public string Type
  {
    get => (string)Data[nameof(Type)]!;
    private set => Data[nameof(Type)] = value;
  }
  public string Id
  {
    get => (string)Data[nameof(Id)]!;
    private set => Data[nameof(Id)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public IdentifierAlreadyUsedException(Type type, string id, string? propertyName = null) : base(BuildMessage(type, id, propertyName))
  {
    if (!type.IsSubclassOf(typeof(AggregateRoot)))
    {
      throw new ArgumentException($"The type must be a subclass of '{nameof(AggregateRoot)}'.", nameof(type));
    }

    Type = type.GetNamespaceQualifiedName();
    Id = id;
    PropertyName = propertyName;
  }

  private static string BuildMessage(Type type, string id, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Type), type.GetNamespaceQualifiedName())
    .AddData(nameof(Id), id)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}

public class IdentifierAlreadyUsedException<T> : IdentifierAlreadyUsedException where T : AggregateRoot
{
  public IdentifierAlreadyUsedException(string id, string? propertyName = null) : base(typeof(T), id, propertyName)
  {
  }
}
