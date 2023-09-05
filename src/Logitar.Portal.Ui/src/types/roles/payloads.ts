import type { SearchPayload, SortOption } from "@/types/search";

export type RoleSort = "DisplayName" | "UniqueName" | "UpdatedOn";

export type RoleSortOption = SortOption & {
  field: RoleSort;
};

export type SearchRolesPayload = SearchPayload & {
  realm?: string;
  sort: RoleSortOption[];
};
