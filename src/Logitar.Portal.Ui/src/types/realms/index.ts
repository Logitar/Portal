import type { Aggregate } from "@/types/aggregate";
import type { CustomAttribute } from "@/types/customAttribute";
import type { PasswordSettings, UniqueNameSettings } from "../settings";

export type ClaimMapping = {
  key: string;
  type: string;
  valueType?: string;
};

export type Realm = Aggregate & {
  id: string;
  uniqueName: string;
  displayName?: string;
  description?: string;
  defaultLocale?: string;
  secret: string;
  url?: string;
  requireConfirmedAccount: boolean;
  requireUniqueEmail: boolean;
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
  claimMappings: ClaimMapping[];
  customAttributes: CustomAttribute[];
};
