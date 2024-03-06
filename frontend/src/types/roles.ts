import type { Aggregate } from "@/types/aggregate";
import type { CollectionAction } from "@/types/modifications";
import type { CustomAttribute } from "@/types/customAttributes";
import type { Realm } from "@/types/realms";
import type { SearchPayload, SortOption } from "@/types/search";

export type CreateRolePayload = {
  uniqueName: string;
  displayName?: string;
  description?: string;
  customAttributes: CustomAttribute[];
};

export type ReplaceRolePayload = {
  uniqueName: string;
  displayName?: string;
  description?: string;
  customAttributes: CustomAttribute[];
};

export type Role = Aggregate & {
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

export type RoleSort = "DisplayName" | "UniqueName" | "UpdatedOn";

export type RoleSortOption = SortOption & {
  field: RoleSort;
};

export type SearchRolesPayload = SearchPayload & {
  sort?: RoleSortOption[];
};
