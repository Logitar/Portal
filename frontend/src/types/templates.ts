import type { Aggregate } from "./aggregate";
import type { Realm } from "./realms";
import type { SearchPayload, SortOption } from "./search";

export type Content = {
  type: string;
  text: string;
};

export type CreateTemplatePayload = {
  uniqueKey: string;
  displayName?: string;
  description?: string;
  subject: string;
  content: Content;
};

export type ReplaceTemplatePayload = {
  uniqueKey: string;
  displayName?: string;
  description?: string;
  subject: string;
  content: Content;
};

export type SearchTemplatesPayload = SearchPayload & {
  contentType?: String;
  sort?: TemplateSortOption[];
};

export type Template = Aggregate & {
  uniqueKey: string;
  displayName?: string;
  description?: string;
  subject: string;
  content: Content;
  realm?: Realm;
};

export type TemplateSort = "DisplayName" | "Subject" | "UniqueKey" | "UpdatedOn";

export type TemplateSortOption = SortOption & {
  field: TemplateSort;
};
