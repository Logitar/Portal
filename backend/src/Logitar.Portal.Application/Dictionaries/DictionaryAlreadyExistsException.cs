using Logitar.Portal.Domain.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries;

public class DictionaryAlreadyExistsException : Exception
{
  private const string ErrorMessage = "The specified dictionary already exists.";

  public Guid? TenantId
  {
    get => (Guid?)Data[nameof(TenantId)];
    private set => Data[nameof(TenantId)] = value;
  }
  public Guid DictionaryId
  {
    get => (Guid)Data[nameof(DictionaryId)]!;
    private set => Data[nameof(DictionaryId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public string Locale
  {
    get => (string)Data[nameof(Locale)]!;
    private set => Data[nameof(Locale)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public DictionaryAlreadyExistsException(Dictionary dictionary, DictionaryId conflictId) : base(BuildMessage(dictionary, conflictId))
  {
    TenantId = dictionary.TenantId?.ToGuid();
    DictionaryId = dictionary.EntityId.ToGuid();
    ConflictId = conflictId.EntityId.ToGuid();
    Locale = dictionary.Locale.ToString();
    PropertyName = nameof(Dictionary.Locale);
  }

  private static string BuildMessage(Dictionary dictionary, DictionaryId conflictId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TenantId), dictionary.TenantId?.ToGuid(), "<null>")
    .AddData(nameof(DictionaryId), dictionary.EntityId.ToGuid())
    .AddData(nameof(ConflictId), conflictId.EntityId.ToGuid())
    .AddData(nameof(Locale), dictionary.Locale)
    .AddData(nameof(PropertyName), nameof(Dictionary.Locale))
    .Build();
}
