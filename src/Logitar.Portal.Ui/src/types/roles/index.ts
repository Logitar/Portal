import type { Aggregate } from "@/types/aggregate";
import type { CollectionAction } from "@/types/modifications";
import type { CustomAttribute } from "@/types/customAttributes";
import type { Realm } from "@/types/realms";

export type Role = Aggregate & {
  id: string;
  uniqueName: string;
  displayName?: string;
  description?: string;
  customAttributes: CustomAttribute[];
  realm?: Realm;
};

export type RoleModification = {
  role: string;
  action: CollectionAction;
};
