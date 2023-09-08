import type { Aggregate } from "@/types/aggregate";
import type { CustomAttribute } from "@/types/customAttributes";
import type { Realm } from "@/types/realms/index";
import type { Role } from "@/types/roles/index";

export type ApiKey = Aggregate & {
  id: string;
  xApiKey?: string;
  displayName: string;
  description?: string;
  expiresOn?: string;
  authenticatedOn?: string;
  customAttributes: CustomAttribute[];
  roles: Role[];
  realm?: Realm;
};
