import type { Actor } from "@/types/actor";
import type { Aggregate } from "@/types/aggregate";
import type { CustomAttribute } from "@/types/customAttributes";
import type { Identifier } from "@/types/identifier";
import type { Realm } from "@/types/realms";
import type { Role } from "@/types/roles";

export type Address = Contact & {
  street: string;
  locality: string;
  region?: string;
  postalCode?: string;
  country: string;
  formatted: string;
};

export type AuthenticatedUser = {
  displayName?: string;
  emailAddress?: string;
  picture?: string;
};

export type Contact = {
  isVerified: boolean;
  verifiedBy?: Actor;
  verifiedOn?: string;
};

export type CountrySettings = {
  code: string;
  postalCode?: string;
  regions: string[];
};

export type Email = Contact & {
  address: string;
};

export type PersonNameType = "first" | "last" | "middle" | "nick";

export type Phone = Contact & {
  countryCode?: string;
  number: string;
  extension?: string;
  e164Formatted: string;
};

export type ProfileUpdatedEvent = {
  toast?: boolean;
  user: User;
};

export type User = Aggregate & {
  id: string;
  uniqueName: string;
  hasPassword: boolean;
  passwordChangedBy?: Actor;
  passwordChangedOn?: string;
  disabledBy?: Actor;
  disabledOn?: Date;
  isDisabled: boolean;
  authenticatedOn?: string;
  address?: Address;
  email?: Email;
  phone?: Phone;
  isConfirmed: boolean;
  firstName?: string;
  middleName?: string;
  lastName?: string;
  fullName?: string;
  nickname?: string;
  birthdate?: string;
  gender?: string;
  locale?: string;
  timeZone?: string;
  picture?: string;
  profile?: string;
  website?: string;
  customAttributes: CustomAttribute[];
  identifiers: Identifier[];
  roles: Role[];
  realm?: Realm;
};
