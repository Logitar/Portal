using Logitar.Identity.Domain.Shared;

namespace Logitar.Portal.Domain.Templates;

public class TemplateNotInTenantException : Exception
{
  public const string ErrorMessage = "The specified template is not in the specified tenant.";

  public TemplateId TemplateId
  {
    get => new((string)Data[nameof(TemplateId)]!);
    private set => Data[nameof(TemplateId)] = value.Value;
  }
  public TenantId? ExpectedTenantId
  {
    get => TenantId.TryCreate((string?)Data[nameof(ExpectedTenantId)]);
    private set => Data[nameof(ExpectedTenantId)] = value?.Value;
  }
  public TenantId? ActualTenantId
  {
    get => TenantId.TryCreate((string?)Data[nameof(ActualTenantId)]);
    private set => Data[nameof(ActualTenantId)] = value?.Value;
  }

  public TemplateNotInTenantException(TemplateAggregate template, TenantId? expectedTenant) : base(BuildMessage(template, expectedTenant))
  {
    TemplateId = template.Id;
    ExpectedTenantId = expectedTenant;
    ActualTenantId = template.TenantId;
  }

  private static string BuildMessage(TemplateAggregate template, TenantId? expectedTenant) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TemplateId), template.Id.Value)
    .AddData(nameof(ExpectedTenantId), expectedTenant?.Value, "<null>")
    .AddData(nameof(ActualTenantId), template.TenantId?.Value, "<null>")
    .Build();
}
