import type { Aggregate } from "@/types/aggregate";
import type { Realm } from "@/types/realms";
import type { SearchPayload, SortOption } from "@/types/search";

export type CreateSenderPayload = {
  emailAddress: string;
  displayName?: string;
  description?: string;
  mailgun?: MailgunSettings;
  sendGrid?: SendGridSettings;
};

export type MailgunSettings = {
  apiKey: string;
  domainName: string;
};

export type ReplaceSenderPayload = {
  emailAddress: string;
  displayName?: string;
  description?: string;
  mailgun?: MailgunSettings;
  sendGrid?: SendGridSettings;
};

export type SearchSendersPayload = SearchPayload & {
  provider?: SenderProvider;
  sort?: SenderSortOption[];
};

export type Sender = Aggregate & {
  isDefault: boolean;
  emailAddress: string;
  displayName?: string;
  description?: string;
  provider: SenderProvider;
  mailgun?: MailgunSettings;
  sendGrid?: SendGridSettings;
  realm?: Realm;
};

export type SenderProvider = "Mailgun" | "SendGrid";

export type SenderSort = "DisplayName" | "EmailAddress" | "UpdatedOn";

export type SenderSortOption = SortOption & {
  field: SenderSort;
};

export type SendGridSettings = {
  apiKey: string;
};
