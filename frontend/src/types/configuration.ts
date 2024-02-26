import type { Aggregate } from "./aggregate";
import type { Locale } from "./i18n";
import type { PasswordSettings, UniqueNameSettings } from "./settings";

export type Configuration = Aggregate & {
  defaultLocale?: Locale;
  secret: string;
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
  requireUniqueEmail: boolean;
  loggingSettings: LoggingSettings;
};

export type InitializeConfigurationPayload = {
  defaultLocale: string;
  user: UserPayload;
};

export type IsConfigurationInitialized = {
  isInitialized: boolean;
};

export type LoggingExtent = "ActivityOnly" | "None" | "Full";

export type LoggingSettings = {
  extent: LoggingExtent;
  onlyErrors: boolean;
};

export type ReplaceConfigurationPayload = {
  defaultLocale?: string;
  secret: string;
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
  requireUniqueEmail: boolean;
  loggingSettings: LoggingSettings;
};

export type UserPayload = {
  uniqueName: string;
  password: string;
  emailAddress?: string;
  firstName?: string;
  lastName?: string;
};
