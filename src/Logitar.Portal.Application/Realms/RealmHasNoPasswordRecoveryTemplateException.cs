using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Realms;

public class RealmHasNoPasswordRecoveryTemplateException : Exception
{
  public RealmHasNoPasswordRecoveryTemplateException(RealmAggregate realm) : base($"The realm '{realm}' has no password recovery template.")
  {
    Realm = realm.ToString();
  }

  public string Realm
  {
    get => (string)Data[nameof(Realm)]!;
    private set => Data[nameof(Realm)] = value;
  }
}
