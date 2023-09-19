using Logitar.Portal.Contracts.Senders;

namespace Logitar.Portal.Infrastructure.Messages.Providers;

public class ProviderStrategyNotSupportedException : NotSupportedException
{
  public ProviderStrategyNotSupportedException(ProviderType type)
    : base($"The provider strategy '{type}' is not supported.")
  {
    Type = type;
  }

  public ProviderType Type
  {
    get => (ProviderType)Data[nameof(Type)]!;
    private set => Data[nameof(Type)] = value;
  }
}
