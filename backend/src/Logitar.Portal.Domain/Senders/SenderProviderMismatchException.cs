using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Domain.Senders;

public class SenderProviderMismatchException : Exception
{
  public const string ErrorMessage = "The specified sender provider was not expected.";

  public SenderId SenderId
  {
    get => new((string)Data[nameof(SenderId)]!);
    private set => Data[nameof(SenderId)] = value.Value;
  }
  public SenderProvider ExpectedProvider
  {
    get => (SenderProvider)Data[nameof(ExpectedProvider)]!;
    private set => Data[nameof(ExpectedProvider)] = value;
  }
  public SenderProvider ActualProvider
  {
    get => (SenderProvider)Data[nameof(ActualProvider)]!;
    private set => Data[nameof(ActualProvider)] = value;
  }

  public SenderProviderMismatchException(Sender sender, SenderProvider actualProvider) : base(BuildMessage(sender, actualProvider))
  {
    SenderId = sender.Id;
    ExpectedProvider = sender.Provider;
    ActualProvider = actualProvider;
  }

  private static string BuildMessage(Sender sender, SenderProvider actualProvider) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(SenderId), sender.Id.Value)
    .AddData(nameof(ExpectedProvider), sender.Provider)
    .AddData(nameof(ActualProvider), actualProvider)
    .Build();
}
