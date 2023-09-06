import type { CustomAttribute, CustomAttributeModification } from "@/types/customAttributes";
import type { Modification } from "@/types/modifications";
import type { RoleModification } from "@/types/roles";
import type { SearchPayload, SortOption } from "@/types/search";

export type AddressPayload = {
  street: string;
  locality: string;
  region?: string;
  postalCode?: string;
  country: string;
  isVerified: boolean;
};

export type ChangePasswordPayload = {
  currentPassword?: string;
  newPassword: string;
};

export type CreateUserPayload = {
  realm?: string;
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

export type EmailPayload = {
  address: string;
  isVerified: boolean;
};

export type PhonePayload = {
  countryCode?: string;
  number: string;
  extension?: string;
  isVerified: boolean;
};

export type SearchUsersPayload = SearchPayload & {
  realm?: string;
  hasPassword?: boolean;
  isConfirmed?: boolean;
  isDisabled?: boolean;
  sort: UserSortOption[];
};

export type SignInPayload = {
  uniqueName: string;
  password: string;
  remember: boolean;
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
