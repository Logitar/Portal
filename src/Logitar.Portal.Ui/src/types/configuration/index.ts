import type { Aggregate } from "@/types/aggregate";
import type { PasswordSettings, UniqueNameSettings } from "../settings";

export type Configuration = Aggregate & {
  defaultLocale: string;
  secret: string;
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
  loggingSettings: LoggingSettings;
};

export type IsConfigurationInitializedResult = {
  isInitialized: boolean;
};

export type LoggingSettings = {
  extent: string;
  onlyErrors: boolean;
};
