using Logitar.Portal.Contracts.Constants;

namespace Logitar.Portal.Application.Realms;

public class RealmNotFoundException : Exception
{
  public const string ErrorMessage = "The specified realm could not be found.";

  public string Realm
  {
    get => (string)Data[nameof(Realm)]!;
    private set => Data[nameof(Realm)] = value;
  }
  public string Header
  {
    get => (string)Data[nameof(Header)]!;
    private set => Data[nameof(Header)] = value;
  }

  public RealmNotFoundException(string realm) : base(BuildMessage(realm))
  {
    Realm = realm;
    Header = Headers.Realm;
  }

  private static string BuildMessage(string realm) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Realm), realm)
    .AddData(nameof(Header), Headers.Realm)
    .Build();
}
