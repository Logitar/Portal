import type { Aggregate } from "@/types/aggregate";
import type { CollectionAction } from "@/types/modifications";
import type { CustomAttribute, CustomAttributeModification } from "@/types/customAttributes";
import type { Modification } from "@/types/modifications";
import type { Realm } from "@/types/realms";
import type { SearchPayload, SortOption } from "@/types/search";

export type CreateRolePayload = {
  realm?: string;
  uniqueName: string;
  displayName?: string;
  description?: string;
  customAttributes: CustomAttribute[];
};

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

export type RoleSort = "DisplayName" | "UniqueName" | "UpdatedOn";

export type RoleSortOption = SortOption & {
  field: RoleSort;
};

export type SearchRolesPayload = SearchPayload & {
  realm?: string;
  sort?: RoleSortOption[];
};

export type UpdateRolePayload = {
  uniqueName?: string;
  displayName?: Modification<string>;
  description?: Modification<string>;
  customAttributes?: CustomAttributeModification[];
};
