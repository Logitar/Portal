import type { ClaimMapping } from ".";
import type { CustomAttribute } from "@/types/customAttribute";
import type { PasswordSettings, UniqueNameSettings } from "@/types/settings";

export type CreateRealmPayload = {
  uniqueName: string;
  displayName?: string;
  description?: string;
  defaultLocale?: string;
  secret?: string;
  url?: string;
  requireConfirmedAccount: boolean;
  requireUniqueEmail: boolean;
  uniqueNameSettings?: UniqueNameSettings;
  passwordSettings?: PasswordSettings;
  claimMappings?: ClaimMapping[];
  customAttributes?: CustomAttribute[];
};

export type UpdateRealmPayload = {
  displayName?: string;
  description?: string;
  defaultLocale?: string;
  secret?: string;
  url?: string;
  requireConfirmedAccount: boolean;
  requireUniqueEmail: boolean;
  uniqueNameSettings?: UniqueNameSettings;
  passwordSettings?: PasswordSettings;
  claimMappings?: ClaimMapping[];
  customAttributes?: CustomAttribute[];
};
