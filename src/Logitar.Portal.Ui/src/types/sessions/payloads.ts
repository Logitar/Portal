import type { SearchPayload, SortOption } from "@/types/search";

export type SearchSessionsPayload = SearchPayload & {
  realm?: string;
  userId?: string;
  isActive?: boolean;
  isPersistent?: boolean;
  sort?: SessionSortOption[];
};

export type SessionSort = "SignedOutOn" | "UpdatedOn";

export type SessionSortOption = SortOption & {
  field: SessionSort;
};
