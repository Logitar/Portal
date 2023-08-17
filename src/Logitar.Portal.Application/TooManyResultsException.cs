using Logitar.EventSourcing;
using Logitar.Portal.Contracts;

namespace Logitar.Portal.Application;

public class TooManyResultsException : Exception
{
  public TooManyResultsException(Type type, int expected, int actual)
    : base(BuildMessage(type, expected, actual))
  {
    if (!type.IsSubclassOf(typeof(Aggregate)))
    {
      throw new ArgumentException($"The type must be a subclass of the '{nameof(Aggregate)}' type.", nameof(type));
    }

    TypeName = type.GetName();
    Expected = expected;
    Actual = actual;
  }

  public string TypeName
  {
    get => (string)Data[nameof(TypeName)]!;
    private set => Data[nameof(TypeName)] = value;
  }
  public int Expected
  {
    get => (int)Data[nameof(Expected)]!;
    private set => Data[nameof(Expected)] = value;
  }
  public int Actual
  {
    get => (int)Data[nameof(Actual)]!;
    private set => Data[nameof(Actual)] = value;
  }

  private static string BuildMessage(Type type, int expected, int actual)
  {
    StringBuilder message = new();

    message.AppendLine("There are too many results.");
    message.Append("Type: ").AppendLine(type.GetName());
    message.Append("Expected: ").Append(expected).AppendLine();
    message.Append("Actual: ").Append(actual).AppendLine();

    return message.ToString();
  }
}

public class TooManyResultsException<T> : TooManyResultsException where T : Aggregate
{
  public TooManyResultsException(int expected, int actual) : base(typeof(T), expected, actual)
  {
  }
}
