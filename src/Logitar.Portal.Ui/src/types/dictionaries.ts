import type { Aggregate } from "@/types/aggregate";
import type { Locale } from "@/types/i18n";
import type { Realm } from "@/types/realms";
import type { SearchPayload, SortOption } from "@/types/search";

export type CreateDictionaryPayload = {
  realm?: string;
  locale: string;
  entries?: DictionaryEntry[];
};

export type Dictionary = Aggregate & {
  id: string;
  realm?: Realm;
  locale: Locale;
  entries: DictionaryEntry[];
  entryCount: number;
};

export type DictionaryEntry = {
  key: string;
  value: string;
};

export type DictionaryEntryModification = {
  key: string;
  value?: string;
};

export type DictionarySort = "Locale" | "UpdatedOn";

export type DictionarySortOption = SortOption & {
  field: DictionarySort;
};

export type SearchDictionariesPayload = SearchPayload & {
  realm?: string;
  sort?: DictionarySortOption[];
};

export type UpdateDictionaryPayload = {
  entries: DictionaryEntryModification[];
};
