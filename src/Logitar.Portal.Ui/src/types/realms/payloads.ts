import type { ClaimMapping } from ".";
import type { CustomAttribute, CustomAttributeModification } from "@/types/customAttributes";
import type { PasswordSettings, UniqueNameSettings } from "@/types/settings";
import type { Modification } from "@/types/modifications";
import type { SearchPayload, SortOption } from "@/types/search";

export type ClaimMappingModification = {
  key: string;
  name?: string;
  type?: string;
};

export type CreateRealmPayload = {
  uniqueSlug: string;
  displayName?: string;
  description?: string;
  defaultLocale?: string;
  secret?: string;
  url?: string;
  requireUniqueEmail: boolean;
  requireConfirmedAccount: boolean;
  uniqueNameSettings?: UniqueNameSettings;
  passwordSettings?: PasswordSettings;
  claimMappings?: ClaimMapping[];
  customAttributes?: CustomAttribute[];
};

export type RealmSort = "DisplayName" | "UniqueSlug" | "UpdatedOn";

export type RealmSortOption = SortOption & {
  field: RealmSort;
};

export type SearchRealmsPayload = SearchPayload & {
  sort: RealmSortOption[];
};

export type UpdateRealmPayload = {
  uniqueSlug?: string;
  displayName?: Modification<string>;
  description?: Modification<string>;
  defaultLocale?: Modification<string>;
  secret?: string;
  url?: Modification<string>;
  requireUniqueEmail?: boolean;
  requireConfirmedAccount?: boolean;
  uniqueNameSettings?: UniqueNameSettings;
  passwordSettings?: PasswordSettings;
  claimMappings?: ClaimMappingModification[];
  customAttributes?: CustomAttributeModification[];
};
