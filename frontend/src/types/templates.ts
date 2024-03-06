import type { Aggregate } from "@/types/aggregate";
import type { Modification } from "@/types/modifications";
import type { Realm } from "@/types/realms";
import type { SearchPayload, SortOption } from "@/types/search";

export type CreateTemplatePayload = {
  realm?: string;
  uniqueName: string;
  displayName?: string;
  description?: string;
  subject: string;
  contentType: string;
  contents: string;
};

export type SearchTemplatesPayload = SearchPayload & {
  realm?: string;
  contentType?: String;
  sort?: TemplateSortOption[];
};

export type Template = Aggregate & {
  id: string;
  uniqueName: string;
  displayName?: string;
  description?: string;
  subject: string;
  contentType: string;
  contents: string;
  realm?: Realm;
};

export type TemplateSort = "DisplayName" | "UniqueName" | "UpdatedOn";

export type TemplateSortOption = SortOption & {
  field: TemplateSort;
};

export type UpdateTemplatePayload = {
  uniqueName?: string;
  displayName?: Modification<string>;
  description?: Modification<string>;
  subject?: string;
  contentType?: String;
  contents?: String;
};
