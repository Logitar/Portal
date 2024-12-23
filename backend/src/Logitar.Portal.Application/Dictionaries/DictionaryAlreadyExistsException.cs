using Logitar.Identity.Core;

namespace Logitar.Portal.Application.Dictionaries;

public class DictionaryAlreadyExistsException : Exception
{
  public const string ErrorMessage = "The specified dictionary already exists.";

  public string? TenantId
  {
    get => (string?)Data[nameof(TenantId)];
    private set => Data[nameof(TenantId)] = value;
  }
  public string Locale
  {
    get => (string)Data[nameof(Locale)]!;
    private set => Data[nameof(Locale)] = value;
  }

  public DictionaryAlreadyExistsException(TenantId? tenantId, Locale locale) : base(BuildMessage(tenantId, locale))
  {
    TenantId = tenantId?.Value;
    Locale = locale.Code;
  }

  private static string BuildMessage(TenantId? tenantId, Locale locale) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TenantId), tenantId, "<null>")
    .AddData(nameof(Locale), locale)
    .Build();
}
