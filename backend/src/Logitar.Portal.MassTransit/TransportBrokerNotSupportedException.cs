using Logitar.Portal.MassTransit.Settings;

namespace Logitar.Portal.MassTransit;

internal class TransportBrokerNotSupportedException : NotSupportedException
{
  public const string ErrorMessage = "The specified transport broker is not supported.";

  public TransportBroker TransportBroker
  {
    get => (TransportBroker)Data[nameof(TransportBroker)]!;
    private set => Data[nameof(TransportBroker)] = value;
  }

  public TransportBrokerNotSupportedException(TransportBroker transportBroker) : base(BuildMessage(transportBroker))
  {
    TransportBroker = transportBroker;
  }

  private static string BuildMessage(TransportBroker transportBroker) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TransportBroker), transportBroker)
    .Build();
}
