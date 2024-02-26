import type { Aggregate } from "./aggregate";
import type { CustomAttribute } from "./customAttributes";
import type { Realm } from "./realms";
import type { SearchPayload, SortOption } from "./search";

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

export type RoleSort = "DisplayName" | "UniqueName" | "UpdatedOn";

export type RoleSortOption = SortOption & {
  field: RoleSort;
};

export type SearchRolesPayload = SearchPayload & {
  sort?: RoleSortOption[];
};
