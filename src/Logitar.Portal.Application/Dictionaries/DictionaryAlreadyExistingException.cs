using FluentValidation.Results;
using Logitar.Portal.Domain;

namespace Logitar.Portal.Application.Dictionaries;

public class DictionaryAlreadyExistingException : Exception, IFailureException
{
  private const string ErrorMessage = "The specified dictionary already exists.";

  public DictionaryAlreadyExistingException(string? tenantId, Locale locale, string propertyName)
    : base(BuildMessage(tenantId, locale, propertyName))
  {
    TenantId = tenantId;
    LocaleCode = locale.Code;
    PropertyName = propertyName;
  }

  public string? TenantId
  {
    get => (string?)Data[nameof(TenantId)];
    private set => Data[nameof(TenantId)] = value;
  }
  public string LocaleCode
  {
    get => (string)Data[nameof(LocaleCode)]!;
    private set => Data[nameof(LocaleCode)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public ValidationFailure Failure => new(PropertyName, ErrorMessage, LocaleCode)
  {
    ErrorCode = "DictionaryAlreadyExisting",
    CustomState = new { TenantId }
  };

  private static string BuildMessage(string? tenantId, Locale locale, string propertyName)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.Append("TenantId: ").AppendLine(tenantId ?? "<null>");
    message.Append("LocaleCode: ").AppendLine(locale.Code);
    message.Append("PropertyName: ").AppendLine(propertyName);

    return message.ToString();
  }
}
