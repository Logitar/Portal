import type { Aggregate } from "./aggregate";
import type { Realm } from "./realms";
import type { SearchPayload, SortOption } from "./search";

export type CreateSenderPayload = {
  emailAddress: string;
  displayName?: string;
  description?: string;
  // TODO(fpion): Mailgun
  // TODO(fpion): SendGrid
};

export type ReplaceSenderPayload = {
  emailAddress: string;
  displayName?: string;
  description?: string;
  // TODO(fpion): Mailgun
  // TODO(fpion): SendGrid
};

export type SearchSendersPayload = SearchPayload & {
  provider?: SenderProvider;
  sort?: SenderSortOption[];
};

export type SenderProvider = "Mailgun" | "SendGrid";

export type Sender = Aggregate & {
  isDefault: boolean;
  emailAddress: string;
  displayName?: string;
  description?: string;
  provider: SenderProvider;
  // TODO(fpion): Mailgun
  // TODO(fpion): SendGrid
  realm?: Realm;
};

export type SenderSort = "DisplayName" | "EmailAddress" | "UpdatedOn";

export type SenderSortOption = SortOption & {
  field: SenderSort;
};
