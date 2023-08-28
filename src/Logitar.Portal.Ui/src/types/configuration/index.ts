import type { Aggregate } from "@/types/aggregate";
import type { PasswordSettings, UniqueNameSettings } from "@/types/settings";
import type { UserProfile } from "../users";

export type Configuration = Aggregate & {
  defaultLocale: string;
  secret: string;
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
  loggingSettings: LoggingSettings;
};

export type InitializeConfigurationResult = {
  configuration: Configuration;
  user: UserProfile;
};

export type IsConfigurationInitializedResult = {
  isInitialized: boolean;
};

export type LoggingSettings = {
  extent: string;
  onlyErrors: boolean;
};
