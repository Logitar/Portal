import type { Actor } from "@/types/actor";
import type { Aggregate } from "@/types/aggregate";
import type { CustomAttribute } from "@/types/customAttributes";
import type { SearchPayload, SortOption } from "@/types/search";
import type { User } from "@/types/users";

export type SearchSessionsPayload = SearchPayload & {
  userId?: string;
  isActive?: boolean;
  isPersistent?: boolean;
  sort?: SessionSortOption[];
};

export type Session = Aggregate & {
  isPersistent: boolean;
  refreshToken?: string;
  isActive: boolean;
  signedOutBy?: Actor;
  signedOutOn?: string;
  customAttributes: CustomAttribute[];
  user: User;
};

export type SessionSort = "SignedOutOn" | "UpdatedOn";

export type SessionSortOption = SortOption & {
  field: SessionSort;
};
