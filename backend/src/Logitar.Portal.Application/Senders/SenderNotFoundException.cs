namespace Logitar.Portal.Application.Senders;

public class SenderNotFoundException : Exception
{
  public const string ErrorMessage = "The specified sender could not be found.";

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

  public SenderNotFoundException(string id, string? propertyName = null) : base(BuildMessage(id, propertyName))
  {
    Id = id;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string id, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Id), id)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
