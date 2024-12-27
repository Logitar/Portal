using Logitar.Portal.Domain.Templates;

namespace Logitar.Portal.Application.Templates;

public class UniqueKeyAlreadyUsedException : Exception
{
  private const string ErrorMessage = "The specified unique key is already used.";

  public Guid? TenantId
  {
    get => (Guid?)Data[nameof(TenantId)];
    private set => Data[nameof(TenantId)] = value;
  }
  public Guid TemplateId
  {
    get => (Guid)Data[nameof(TemplateId)]!;
    private set => Data[nameof(TemplateId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public string UniqueKey
  {
    get => (string)Data[nameof(UniqueKey)]!;
    private set => Data[nameof(UniqueKey)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public UniqueKeyAlreadyUsedException(Template template, TemplateId conflictId) : base(BuildMessage(template, conflictId))
  {
    TenantId = template.TenantId?.ToGuid();
    TemplateId = template.EntityId.ToGuid();
    ConflictId = conflictId.EntityId.ToGuid();
    UniqueKey = template.UniqueKey.Value;
    PropertyName = nameof(Template.UniqueKey);
  }

  private static string BuildMessage(Template template, TemplateId conflictId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TenantId), template.TenantId?.ToGuid(), "<null>")
    .AddData(nameof(TemplateId), template.EntityId.ToGuid())
    .AddData(nameof(ConflictId), conflictId.EntityId.ToGuid())
    .AddData(nameof(UniqueKey), template.UniqueKey.Value)
    .AddData(nameof(PropertyName), nameof(Template.UniqueKey))
    .Build();
}
