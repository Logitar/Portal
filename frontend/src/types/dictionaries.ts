import type { Aggregate } from "@/types/aggregate";
import type { Locale } from "@/types/i18n";
import type { Realm } from "@/types/realms";
import type { SearchPayload, SortOption } from "@/types/search";

export type CreateDictionaryPayload = {
  locale: string;
};

export type Dictionary = Aggregate & {
  locale: Locale;
  entryCount: number;
  entries: DictionaryEntry[];
  realm?: Realm;
};

export type DictionaryEntry = {
  key: string;
  value: string;
};

export type DictionarySort = "EntryCount" | "Locale" | "UpdatedOn";

export type DictionarySortOption = SortOption & {
  field: DictionarySort;
};

export type ReplaceDictionaryPayload = {
  locale: string;
  entries: DictionaryEntry[];
};

export type SearchDictionariesPayload = SearchPayload & {
  isEmpty?: boolean;
  sort?: DictionarySortOption[];
};
