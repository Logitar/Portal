import type { Aggregate } from "./aggregate";
import type { CustomAttribute } from "./customAttributes";
import type { Locale } from "./i18n";
import type { PasswordSettings, UniqueNameSettings } from "./settings";
import type { SearchPayload, SortOption } from "./search";

export type CreateRealmPayload = {
  uniqueSlug: string;
  displayName?: string;
  description?: string;
  defaultLocale?: string;
  secret?: string;
  url?: string;
  uniqueNameSettings?: UniqueNameSettings;
  passwordSettings?: PasswordSettings;
  requireUniqueEmail: boolean;
  customAttributes: CustomAttribute[];
};

export type Realm = Aggregate & {
  uniqueSlug: string;
  displayName?: string;
  description?: string;
  defaultLocale?: Locale;
  secret: string;
  url?: string;
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
  requireUniqueEmail: boolean;
  customAttributes: CustomAttribute[];
};

export type RealmSort = "DisplayName" | "UniqueSlug" | "UpdatedOn";

export type RealmSortOption = SortOption & {
  field: RealmSort;
};

export type ReplaceRealmPayload = {
  uniqueSlug: string;
  displayName?: string;
  description?: string;
  defaultLocale?: string;
  secret: string;
  url?: string;
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
  requireUniqueEmail: boolean;
  customAttributes: CustomAttribute[];
};

export type SearchRealmsPayload = SearchPayload & {
  sort?: RealmSortOption[];
};
