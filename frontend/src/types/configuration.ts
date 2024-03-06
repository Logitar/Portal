import type { Aggregate } from "@/types/aggregate";
import type { Locale } from "@/types/i18n";
import type { PasswordSettings, UniqueNameSettings } from "@/types/settings";

export type Configuration = Aggregate & {
  defaultLocale?: Locale;
  secret: string;
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
  requireUniqueEmail: boolean;
  loggingSettings: LoggingSettings;
};

export type LoggingExtent = "ActivityOnly" | "Full" | "None";

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
