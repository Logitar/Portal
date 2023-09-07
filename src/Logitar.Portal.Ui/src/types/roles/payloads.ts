import type { CustomAttribute, CustomAttributeModification } from "@/types/customAttributes";
import type { Modification } from "@/types/modifications";
import type { SearchPayload, SortOption } from "@/types/search";

export type CreateRolePayload = {
  realm?: string;
  uniqueName: string;
  displayName?: string;
  description?: string;
  customAttributes: CustomAttribute[];
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
