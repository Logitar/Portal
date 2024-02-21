namespace Logitar.Portal.Application.Templates;

public class TemplateNotFoundException : Exception
{
  public const string ErrorMessage = "The specified template could not be found.";

  public string Identifier
  {
    get => (string)Data[nameof(Identifier)]!;
    private set => Data[nameof(Identifier)] = value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public TemplateNotFoundException(string identifier, string? propertyName = null) : base(BuildMessage(identifier, propertyName))
  {
    Identifier = identifier;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string identifier, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Identifier), identifier)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
