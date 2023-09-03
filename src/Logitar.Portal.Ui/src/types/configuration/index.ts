import type { Aggregate } from "@/types/aggregate";
import type { PasswordSettings, UniqueNameSettings } from "@/types/settings";
import type { User } from "@/types/users";

export type Configuration = Aggregate & {
  defaultLocale: string;
  secret: string;
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
  loggingSettings: LoggingSettings;
};

export type InitializeConfigurationResult = {
  configuration: Configuration;
  user: User;
};

export type IsConfigurationInitializedResult = {
  isInitialized: boolean;
};

export type LoggingSettings = {
  extent: string;
  onlyErrors: boolean;
};
