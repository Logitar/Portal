using Logitar.Portal.Core.Emails.Templates;

namespace Logitar.Portal.Core.Realms
{
  public class PasswordRecoveryTemplate
  {
    public PasswordRecoveryTemplate(Realm realm, Template template)
    {
      Realm = realm ?? throw new ArgumentNullException(nameof(realm));
      RealmSid = realm.Sid;
      Template = template ?? throw new ArgumentNullException(nameof(template));
      TemplateSid = template.Sid;
    }
    private PasswordRecoveryTemplate()
    {
    }

    public Realm? Realm { get; set; }
    public int RealmSid { get; set; }

    public Template? Template { get; set; }
    public int TemplateSid { get; set; }
  }
}
