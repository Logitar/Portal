using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Domain.Senders;

public class SenderProviderMismatchException : Exception
{
  public const string ErrorMessage = "The specified sender provider was not expected.";

  public Guid? TenantId
  {
    get => (Guid)Data[nameof(TenantId)]!;
    private set => Data[nameof(TenantId)] = value;
  }
  public Guid SenderId
  {
    get => (Guid)Data[nameof(SenderId)]!;
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

  public SenderProviderMismatchException(Sender sender, SenderProvider actualProvider) : base(BuildMessage(sender, actualProvider))
  {
    TenantId = sender.TenantId?.ToGuid();
    SenderId = sender.EntityId.ToGuid();
    ExpectedProvider = sender.Provider;
    ActualProvider = actualProvider;
  }

  private static string BuildMessage(Sender sender, SenderProvider actualProvider) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TenantId), sender.TenantId?.ToGuid())
    .AddData(nameof(SenderId), sender.EntityId.ToGuid())
    .AddData(nameof(ExpectedProvider), sender.Provider)
    .AddData(nameof(ActualProvider), actualProvider)
    .Build();
}
