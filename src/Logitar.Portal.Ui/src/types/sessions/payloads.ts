import type { CustomAttribute } from "@/types/customAttributes";
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

export type SignInPayload = {
  realm?: string;
  uniqueName: string;
  password: string;
  isPersistent?: boolean;
  customAttributes?: CustomAttribute[];
};
