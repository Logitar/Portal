using Logitar.Portal.Domain.Templates;

namespace Logitar.Portal.Application.Realms;

public class UniqueKeyAlreadyUsedException : Exception
{
  public const string ErrorMessage = "The specified unique key is already used.";

  public UniqueKeyUnit UniqueKey
  {
    get => new((string)Data[nameof(UniqueKey)]!);
    private set => Data[nameof(UniqueKey)] = value.Value;
  }

  public UniqueKeyAlreadyUsedException(UniqueKeyUnit uniqueKey) : base(BuildMessage(uniqueKey))
  {
    UniqueKey = uniqueKey;
  }

  private static string BuildMessage(UniqueKeyUnit uniqueKey) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(UniqueKey), uniqueKey.Value)
    .Build();
}
