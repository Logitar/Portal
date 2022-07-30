namespace Portal.Core.Emails.Messages.Payloads
{
  public class SendDemoMessagePayload
  {
    public Guid TemplateId { get; set; }

    public IEnumerable<VariablePayload>? Variables { get; set; }
  }
}
