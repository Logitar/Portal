import type { Aggregate } from "@/types/aggregate";
import type { Locale } from "@/types/i18n";
import type { Realm } from "@/types/realms";
import type { SearchPayload, SortOption } from "@/types/search";
import type { Sender } from "@/types/senders";
import type { Template } from "@/types/templates";
import type { User } from "@/types/users";

export type Message = Aggregate & {
  id: string;
  subject: string;
  body: string;
  recipients: Recipient[];
  recipientCount: number;
  realm?: Realm;
  sender: Sender;
  template: Template;
  ignoreUserLocale: boolean;
  locale?: Locale;
  variables: Variable[];
  isDemo: boolean;
  result: ResultData[];
  status: MessageStatus;
};

export type MessageSort = "RecipientCount" | "Subject" | "UpdatedOn";

export type MessageSortOption = SortOption & {
  field: MessageSort;
};

export type MessageStatus = "Failed" | "Succeeded" | "Unsent";

export type Recipient = {
  type: RecipientType;
  user?: User;
  address: string;
  displayName?: string;
};

export type RecipientType = "Bcc" | "CC" | "To";

export type ResultData = {
  key: string;
  value: string;
};

export type SearchMessagesPayload = SearchPayload & {
  realm?: string;
  isDemo?: boolean;
  status?: MessageStatus;
  template?: string;
  sort?: MessageSortOption[];
};

export type SendDemoMessagePayload = {
  senderId?: string;
  templateId: string;
  locale?: string;
  variables: Variable[];
};

export type Variable = {
  key: string;
  value: string;
};
