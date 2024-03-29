import type { Aggregate } from "@/types/aggregate";
import type { Realm } from "@/types/realms";
import type { SearchPayload, SortOption } from "@/types/search";

export type Content = {
  type: ContentType;
  text: string;
};

export type ContentType = "text/html" | "text/plain";

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
