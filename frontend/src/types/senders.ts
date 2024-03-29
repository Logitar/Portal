import type { Aggregate } from "@/types/aggregate";
import type { Realm } from "@/types/realms";
import type { SearchPayload, SortOption } from "@/types/search";

export type CreateSenderPayload = {
  emailAddress?: string;
  phoneNumber?: string;
  displayName?: string;
  description?: string;
  mailgun?: MailgunSettings;
  sendGrid?: SendGridSettings;
  twilio?: TwilioSettings;
};

export type MailgunSettings = {
  apiKey: string;
  domainName: string;
};

export type ReplaceSenderPayload = {
  emailAddress?: string;
  phoneNumber?: string;
  displayName?: string;
  description?: string;
  mailgun?: MailgunSettings;
  sendGrid?: SendGridSettings;
  twilio?: TwilioSettings;
};

export type SearchSendersPayload = SearchPayload & {
  provider?: SenderProvider;
  sort?: SenderSortOption[];
};

export type Sender = Aggregate & {
  isDefault: boolean;
  emailAddress?: string;
  phoneNumber?: string;
  displayName?: string;
  description?: string;
  provider: SenderProvider;
  mailgun?: MailgunSettings;
  sendGrid?: SendGridSettings;
  twilio?: TwilioSettings;
  realm?: Realm;
};

export type SenderProvider = "Mailgun" | "SendGrid" | "Twilio";

export type SenderSort = "DisplayName" | "EmailAddress" | "UpdatedOn";

export type SenderSortOption = SortOption & {
  field: SenderSort;
};

export type SenderType = "Email" | "Sms";

export type SendGridSettings = {
  apiKey: string;
};

export type TwilioSettings = {
  accountSid: string;
  authenticationToken: string;
};
