namespace Logitar.Portal.MassTransit.Settings;

internal class TransportBrokerNotSupportedException : NotSupportedException
{
  private const string ErrorMessage = "The specified transport broker is not supported.";

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
