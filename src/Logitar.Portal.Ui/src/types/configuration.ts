import type { Aggregate } from "@/types/aggregate";
import type { CustomAttribute } from "@/types/customAttributes";
import type { PasswordSettings, UniqueNameSettings } from "@/types/settings";
import type { Session } from "@/types/sessions";

export type Configuration = Aggregate & {
  defaultLocale: string;
  secret: string;
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
  loggingSettings: LoggingSettings;
};

export type InitializeConfigurationPayload = {
  locale: string;
  user: UserPayload;
  session?: SessionPayload;
};

export type InitializeConfigurationResult = {
  configuration: Configuration;
  session: Session;
};

export type IsConfigurationInitializedResult = {
  isInitialized: boolean;
};

export type LoggingSettings = {
  extent: string;
  onlyErrors: boolean;
};

export type SessionPayload = {
  isPersistent: boolean;
  customAttributes: CustomAttribute[];
};

export type UpdateConfigurationPayload = {
  defaultLocale?: string;
  secret?: string;
  uniqueNameSettings?: UniqueNameSettings;
  passwordSettings?: PasswordSettings;
  loggingSettings?: LoggingSettings;
};

export type UserPayload = {
  uniqueName: string;
  password: string;
  emailAddress: string;
  firstName: string;
  lastName: string;
};
