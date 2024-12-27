using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Realms;

public class UniqueSlugAlreadyUsedException : Exception
{
  private const string ErrorMessage = "The specified unique slug is already used.";

  public Guid RealmId
  {
    get => (Guid)Data[nameof(RealmId)]!;
    private set => Data[nameof(RealmId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public string Slug
  {
    get => (string)Data[nameof(Slug)]!;
    private set => Data[nameof(Slug)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public UniqueSlugAlreadyUsedException(Realm realm, RealmId conflictId) : base(BuildMessage(realm, conflictId))
  {
  }

  private static string BuildMessage(Realm realm, RealmId conflictId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RealmId), realm.Id.ToGuid())
    .AddData(nameof(ConflictId), conflictId.ToGuid())
    .AddData(nameof(Slug), realm.UniqueSlug)
    .AddData(nameof(PropertyName), nameof(Realm.UniqueSlug))
    .Build();
}
