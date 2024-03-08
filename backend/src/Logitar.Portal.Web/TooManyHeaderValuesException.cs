namespace Logitar.Portal.Web;

public class TooManyHeaderValuesException : Exception
{
  public const string ErrorMessage = "There are too many header values.";

  public string Header
  {
    get => (string)Data[nameof(Header)]!;
    private set => Data[nameof(Header)] = value;
  }
  public int ExpectedCount
  {
    get => (int)Data[nameof(ExpectedCount)]!;
    private set => Data[nameof(ExpectedCount)] = value;
  }
  public int ActualCount
  {
    get => (int)Data[nameof(ActualCount)]!;
    private set => Data[nameof(ActualCount)] = value;
  }

  public TooManyHeaderValuesException(string header, int expectedCount, int actualCount) : base(BuildMessage(header, expectedCount, actualCount))
  {
    Header = header;
    ExpectedCount = expectedCount;
    ActualCount = actualCount;
  }

  private static string BuildMessage(string header, int expectedCount, int actualCount) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Header), header)
    .AddData(nameof(ExpectedCount), expectedCount)
    .AddData(nameof(ActualCount), actualCount)
    .Build();
}
