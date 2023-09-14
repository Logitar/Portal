using FluentValidation.Results;

namespace Logitar.Portal.Application.Messages;

public class MissingRecipientAddressesException : Exception
{
  private const string ErrorMessage = "The specified recipients are missing an email address.";

  public MissingRecipientAddressesException(IEnumerable<string> recipients, string propertyName)
  {
    Recipients = recipients;
    PropertyName = propertyName;
  }

  public IEnumerable<string> Recipients
  {
    get => (IEnumerable<string>)Data[nameof(Recipients)]!;
    private set => Data[nameof(Recipients)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public ValidationFailure Failure => new(PropertyName, ErrorMessage, Recipients)
  {
    ErrorCode = "MissingRecipientAddresses"
  };
}
