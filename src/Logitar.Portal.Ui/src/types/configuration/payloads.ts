import type { LoggingSettings } from ".";
import type { PasswordSettings, UniqueNameSettings } from "@types/settings";

export type InitializeConfigurationPayload = {
  locale: string;
  user: UserPayload;
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
