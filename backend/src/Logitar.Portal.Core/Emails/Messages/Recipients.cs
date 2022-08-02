namespace Logitar.Portal.Core.Emails.Messages
{
  internal class Recipients
  {
    public Recipients(List<Recipient> to, List<Recipient> cc, List<Recipient> bcc)
    {
      To = to ?? throw new ArgumentNullException(nameof(to));
      CC = cc ?? throw new ArgumentNullException(nameof(cc));
      Bcc = bcc ?? throw new ArgumentNullException(nameof(bcc));
    }

    public List<Recipient> To { get; }
    public List<Recipient> CC { get; }
    public List<Recipient> Bcc { get; }
  }
}
