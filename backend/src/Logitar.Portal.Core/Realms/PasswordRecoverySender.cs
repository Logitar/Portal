using Logitar.Portal.Core.Emails.Senders;

namespace Logitar.Portal.Core.Realms
{
  public class PasswordRecoverySender
  {
    public PasswordRecoverySender(Realm realm, Sender sender)
    {
      Realm = realm ?? throw new ArgumentNullException(nameof(realm));
      RealmSid = realm.Sid;
      Sender = sender ?? throw new ArgumentNullException(nameof(sender));
      SenderSid = sender.Sid;
    }
    private PasswordRecoverySender()
    {
    }

    public Realm? Realm { get; private set; }
    public int RealmSid { get; private set; }

    public Sender? Sender { get; private set; }
    public int SenderSid { get; private set; }
  }
}
