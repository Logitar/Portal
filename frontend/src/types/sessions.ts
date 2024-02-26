import type { Actor } from "./actor";
import type { Aggregate } from "./aggregate";
import type { CustomAttribute } from "./customAttributes";
import type { SearchPayload, SortOption } from "./search";
import type { User } from "./users";

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
