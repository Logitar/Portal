import type { Actor } from "@/types/actor";
import type { Aggregate } from "@/types/aggregate";
import type { CustomAttribute, CustomAttributeModification } from "@/types/customAttributes";
import type { CustomIdentifier } from "@/types/customIdentifiers";
import type { Locale } from "@/types/i18n";
import type { Modification } from "@/types/modifications";
import type { Realm } from "@/types/realms";
import type { Role, RoleModification } from "@/types/roles";
import type { SearchPayload, SortOption } from "@/types/search";

export type Address = Contact & {
  street: string;
  locality: string;
  region?: string;
  postalCode?: string;
  country: string;
  formatted: string;
};

export type AddressPayload = ContactPayload & {
  street: string;
  locality: string;
  region?: string;
  postalCode?: string;
  country: string;
};

export type AuthenticatedUser = {
  displayName?: string;
  emailAddress?: string;
  picture?: string;
};

export type ChangePasswordPayload = {
  current?: string;
  new: string;
};

export type Contact = {
  isVerified: boolean;
  verifiedBy?: Actor;
  verifiedOn?: string;
};

export type ContactPayload = {
  isVerified: boolean;
};

export type CountrySettings = {
  code: string;
  postalCode?: string;
  regions: string[];
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

export type PersonNameType = "first" | "last" | "middle" | "nick";

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
};

export type SearchUsersPayload = SearchPayload & {
  hasAuthenticated?: boolean;
  hasPassword?: boolean;
  isConfirmed?: boolean;
  isDisabled?: boolean;
  sort?: UserSortOption[];
};

export type UpdateUserPayload = {
  uniqueName?: string;
  password?: ChangePasswordPayload;
  isDisabled?: boolean;
  address?: Modification<AddressPayload>;
  email?: Modification<EmailPayload>;
  phone?: Modification<PhonePayload>;
  firstName?: Modification<string>;
  middleName?: Modification<string>;
  lastName?: Modification<string>;
  nickname?: Modification<string>;
  birthdate?: Modification<Date>;
  gender?: Modification<string>;
  locale?: Modification<string>;
  timeZone?: Modification<string>;
  picture?: Modification<string>;
  profile?: Modification<string>;
  website?: Modification<string>;
  customAttributes?: CustomAttributeModification[];
  roles?: RoleModification[];
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

export type UserUpdatedEvent = {
  toast?: string;
  user: User;
};
