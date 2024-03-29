import i18n from "@/i18n";
import type { Dictionary } from "@/types/dictionaries";
import type { Realm } from "@/types/realms";
import type { Role } from "@/types/roles";
import type { Sender } from "@/types/senders";
import type { Template } from "@/types/templates";
import type { User } from "@/types/users";

const { t } = i18n.global;

export function formatDictionary(dictionary: Dictionary): string {
  return [dictionary.realm ? formatRealm(dictionary.realm) : t("brand"), dictionary.locale.nativeName].join(" | ");
}

export function formatRealm(realm: Realm): string {
  return realm.displayName ? `${realm.displayName} (${realm.uniqueSlug})` : realm.uniqueSlug;
}

export function formatRole(role: Role): string {
  return role.displayName ? `${role.displayName} (${role.uniqueName})` : role.uniqueName;
}

export function formatSender(sender: Sender): string {
  return (sender.emailAddress ? (sender.displayName ? `${sender.displayName} <${sender.emailAddress}>` : sender.emailAddress) : sender.phoneNumber) ?? "";
}

export function formatTemplate(template: Template): string {
  return template.displayName ? `${template.displayName} (${template.uniqueKey})` : template.uniqueKey;
}

export function formatUser(user: User): string {
  return user.fullName ? `${user.fullName} (${user.uniqueName})` : user.uniqueName;
}
