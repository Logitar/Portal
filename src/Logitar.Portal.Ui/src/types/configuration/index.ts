import type { Aggregate } from "@/types/aggregate";
import type { PasswordSettings, UniqueNameSettings } from "@/types/settings";
import type { Session } from "@/types/sessions";

export type Configuration = Aggregate & {
  defaultLocale: string;
  secret: string;
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
  loggingSettings: LoggingSettings;
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
