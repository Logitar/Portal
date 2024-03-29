import type { Aggregate } from "@/types/aggregate";
import type { Content } from "@/types/templates";
import type { Locale } from "@/types/i18n";
import type { Realm } from "@/types/realms";
import type { SearchPayload, SortOption } from "@/types/search";
import type { Sender } from "@/types/senders";
import type { Template } from "@/types/templates";
import type { User } from "@/types/users";

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
  phoneNumber?: string;
  user?: User;
};

export type RecipientPayload = {
  type: RecipientType;
  address?: string;
  displayName?: string;
  phoneNumber?: string;
  userId?: string;
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

export type SentMessages = {
  ids: string[];
};

export type Variable = {
  key: string;
  value: string;
};
