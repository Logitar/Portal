import type { Aggregate } from "./aggregate";
import type { Locale } from "./i18n";
import type { Realm } from "./realms";
import type { SearchPayload, SortOption } from "./search";
import type { Sender } from "./senders";
import type { Content, Template } from "./templates";
import type { User } from "./users";

export type Message = Aggregate & {
  subject: string;
  body: Content;
  recipientCount: number;
  recipients: Recipient[];
  sender: Sender;
  template: Template;
  ignoreUserLocale: boolean;
  locale?: Locale;
  variables: Variable[];
  isDemo: boolean;
  status: MessageStatus;
  resultData: ResultData[];
  realm?: Realm;
};

export type MessageSort = "RecipientCount" | "Subject" | "UpdatedOn";

export type MessageSortOption = SortOption & {
  field: MessageSort;
};

export type MessageStatus = "Failed" | "Succeeded" | "Unsent";

export type Recipient = {
  type: RecipientType;
  address: string;
  displayName?: string;
  user?: User;
};

export type RecipientPayload = {
  type: RecipientType;
  userId?: string;
  address?: string;
  displayName?: string;
};

export type RecipientType = "Bcc" | "CC" | "To";

export type ResultData = {
  key: string;
  value: string;
};

export type SearchMessagesPayload = SearchPayload & {
  templateId?: string;
  isDemo?: boolean;
  status?: MessageStatus;
  sort?: MessageSortOption[];
};

export type SendMessagePayload = {
  senderId?: string;
  template: string;
  recipients: RecipientPayload[];
  ignoreUserLocale: boolean;
  locale?: string;
  variables: Variable[];
  isDemo: boolean;
};

export type Variable = {
  key: string;
  value: string;
};
