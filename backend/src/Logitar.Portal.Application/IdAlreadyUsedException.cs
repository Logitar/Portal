namespace Logitar.Portal.Application;

public class IdAlreadyUsedException : Exception
{
  private const string ErrorMessage = "The specified identifier is already taken.";

  public Guid Id
  {
    get => (Guid)Data[nameof(Id)]!;
    private set => Data[nameof(Id)] = value;
  }

  public IdAlreadyUsedException(Guid id) : base(BuildMessage(id))
  {
    Id = id;
  }

  private static string BuildMessage(Guid id) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Id), id)
    .Build();
}
