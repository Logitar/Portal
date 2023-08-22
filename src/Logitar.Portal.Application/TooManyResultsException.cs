using Logitar.EventSourcing;

namespace Logitar.Portal.Application;

public class TooManyResultsException : Exception
{
  private const string ErrorMessage = "There are too many results.";

  public TooManyResultsException(Type type, int expected, int actual)
  {
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

    message.AppendLine(ErrorMessage);
    message.Append("TypeName: ").AppendLine(type.GetName());
    message.Append("Expected: ").Append(expected).AppendLine();
    message.Append("Actual: ").Append(actual).AppendLine();

    return message.ToString();
  }
}

public class TooManyResultsException<T> : TooManyResultsException
{
  public TooManyResultsException(int expected, int actual) : base(typeof(T), expected, actual)
  {
  }
}
