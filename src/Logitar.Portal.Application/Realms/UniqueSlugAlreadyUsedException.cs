using FluentValidation.Results;

namespace Logitar.Portal.Application.Realms;

public class UniqueSlugAlreadyUsedException : Exception
{
  private const string ErrorMessage = "The specified unique slug is already used.";

  public UniqueSlugAlreadyUsedException(string uniqueSlug, string propertyName)
    : base(BuildMessage(uniqueSlug, propertyName))
  {
    UniqueSlug = uniqueSlug;
    PropertyName = propertyName;
  }

  public string UniqueSlug
  {
    get => (string)Data[nameof(UniqueSlug)]!;
    private set => Data[nameof(UniqueSlug)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public ValidationFailure Failure => new(PropertyName, ErrorMessage, UniqueSlug)
  {
    ErrorCode = "UniqueSlugAlreadyUsed"
  };

  private static string BuildMessage(string uniqueSlug, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append("UniqueSlug: ").AppendLine(uniqueSlug);
    message.Append("PropertyName: ").AppendLine(propertyName);

    return message.ToString();
  }
}
