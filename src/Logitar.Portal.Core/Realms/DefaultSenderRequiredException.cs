namespace Logitar.Portal.Core.Realms;

public class DefaultSenderRequiredException : Exception
{
  public DefaultSenderRequiredException(RealmAggregate? realm) : base(GetMessage(realm))
  {
    Data["Realm"] = realm?.ToString() ?? "Portal";
  }

  private static string GetMessage(RealmAggregate? realm) => realm == null
    ? "The default sender is required for the Portal."
    : $"The default sender is required for the realm '{realm}'.";
}
