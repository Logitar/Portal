namespace Logitar.Portal.Domain.Senders;

public class SenderTypeNotSupportedException : NotSupportedException
{
  public const string ErrorMessage = "The specified sender type is not supported.";

  public SenderType SenderType
  {
    get => (SenderType)Data[nameof(SenderType)]!;
    private set => Data[nameof(SenderType)] = value;
  }

  public SenderTypeNotSupportedException(SenderType senderType)
    : base(BuildMessage(senderType))
  {
    SenderType = senderType;
  }

  private static string BuildMessage(SenderType senderType) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(SenderType), senderType)
    .Build();
}
