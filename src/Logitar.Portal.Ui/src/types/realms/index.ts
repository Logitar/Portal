import type { Aggregate } from "@/types/aggregate";
import type { CustomAttribute } from "@/types/customAttributes";
import type { PasswordSettings, UniqueNameSettings } from "@/types/settings";

export type ClaimMapping = {
  key: string;
  name: string;
  type?: string;
};

export type Realm = Aggregate & {
  id: string;
  uniqueSlug: string;
  displayName?: string;
  description?: string;
  defaultLocale?: string;
  secret: string;
  url?: string;
  requireUniqueEmail: boolean;
  requireConfirmedAccount: boolean;
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
  claimMappings: ClaimMapping[];
  customAttributes: CustomAttribute[];
};
