import type { Realm } from "@/types/realms";
import type { Role } from "@/types/roles";
import type { Sender } from "@/types/senders";
import type { Template } from "@/types/templates";
import type { User } from "@/types/users";

// TODO(fpion): formatDictionary (#334)

export function formatRealm(realm: Realm): string {
  return realm.displayName ? `${realm.displayName} (${realm.uniqueSlug})` : realm.uniqueSlug;
}

export function formatRole(role: Role): string {
  return role.displayName ? `${role.displayName} (${role.uniqueName})` : role.uniqueName;
}

export function formatSender(sender: Sender): string {
  return sender.displayName ? `${sender.displayName} <${sender.emailAddress}>` : sender.emailAddress;
}

export function formatTemplate(template: Template): string {
  return template.displayName ? `${template.displayName} (${template.uniqueName})` : template.uniqueName;
}

export function formatUser(user: User): string {
  return user.fullName ? `${user.fullName} (${user.uniqueName})` : user.uniqueName;
}
