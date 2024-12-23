using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Domain.Senders;

public class SenderProviderMismatchException : Exception
{
  public const string ErrorMessage = "The specified sender provider was not expected.";

  public string SenderId
  {
    get => (string)Data[nameof(SenderId)]!;
    private set => Data[nameof(SenderId)] = value;
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

  public SenderProviderMismatchException(SenderAggregate sender, SenderProvider actualProvider) : base(BuildMessage(sender, actualProvider))
  {
    SenderId = sender.Id.Value;
    ExpectedProvider = sender.Provider;
    ActualProvider = actualProvider;
  }

  private static string BuildMessage(SenderAggregate sender, SenderProvider actualProvider) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(SenderId), sender.Id.Value)
    .AddData(nameof(ExpectedProvider), sender.Provider)
    .AddData(nameof(ActualProvider), actualProvider)
    .Build();
}
