import type { Actor } from "./actor";
import type { Aggregate } from "./aggregate";
import type { CustomAttribute } from "./customAttributes";
import type { CustomIdentifier } from "./customIdentifiers";
import type { Locale } from "./i18n";
import type { Realm } from "./realms";
import type { Role } from "./roles";
import type { SearchPayload, SortOption } from "./search";

export type Address = Contact & {
  street: string;
  locality: string;
  postalCode?: string;
  region?: string;
  country: string;
  formatted: string;
};

export type AddressPayload = ContactPayload & {
  street: string;
  locality: string;
  postalCode?: string;
  region?: string;
  country: string;
};

export type Contact = {
  isVerified: boolean;
  verifiedBy?: Actor;
  verifiedOn?: string;
};

export type ContactPayload = {
  isVerified: boolean;
};

export type CreateUserPayload = {
  uniqueName: string;
  password?: string;
  isDisabled: boolean;
  address?: AddressPayload;
  email?: EmailPayload;
  phone?: PhonePayload;
  firstName?: string;
  middleName?: string;
  lastName?: string;
  nickname?: string;
  birthdate?: Date;
  gender?: string;
  locale?: string;
  timeZone?: string;
  picture?: string;
  profile?: string;
  website?: string;
  customAttributes: CustomAttribute[];
  customIdentifiers: CustomIdentifier[];
  roles: string[];
};

export type Email = Contact & {
  address: string;
};

export type EmailPayload = ContactPayload & {
  address: string;
};

export type Phone = Contact & {
  countryCode?: string;
  number: string;
  extension?: string;
  e164Formatted: string;
};

export type PhonePayload = ContactPayload & {
  countryCode?: string;
  number: string;
  extension?: string;
  isVerified: boolean;
};

export type ReplaceUserPayload = {
  uniqueName: string;
  password?: string;
  isDisabled: boolean;
  address?: AddressPayload;
  email?: EmailPayload;
  phone?: PhonePayload;
  firstName?: string;
  middleName?: string;
  lastName?: string;
  nickname?: string;
  birthdate?: Date;
  gender?: string;
  locale?: string;
  timeZone?: string;
  picture?: string;
  profile?: string;
  website?: string;
  customAttributes: CustomAttribute[];
  roles: string[];
};

export type SearchUsersPayload = SearchPayload & {
  hasAuthenticated?: boolean;
  hasPassword?: boolean;
  isDisabled?: boolean;
  isConfirmed?: boolean;
  sort?: UserSortOption[];
};

export type User = Aggregate & {
  uniqueName: string;
  hasPassword: boolean;
  passwordChangedBy?: Actor;
  passwordChangedOn?: string;
  disabledBy?: Actor;
  disabledOn?: string;
  isDisabled: boolean;
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
  locale?: Locale;
  timeZone?: string;
  picture?: string;
  profile?: string;
  website?: string;
  authenticatedOn?: string;
  customAttributes: CustomAttribute[];
  customIdentifiers: CustomIdentifier[];
  roles: Role[];
  realm?: Realm;
};

export type UserSort =
  | "AuthenticatedOn"
  | "Birthdate"
  | "DisabledOn"
  | "EmailAddress"
  | "FirstName"
  | "FullName"
  | "LastName"
  | "MiddleName"
  | "Nickname"
  | "PasswordChangedOn"
  | "PhoneNumber"
  | "UniqueName"
  | "UpdatedOn";

export type UserSortOption = SortOption & {
  field: UserSort;
};
