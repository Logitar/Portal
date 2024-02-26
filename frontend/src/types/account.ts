export type AddressPayload = {
  street: string;
  locality: string;
  postalCode?: string;
  region?: string;
  country: string;
};

export type ChangePasswordPayload = {
  current: string;
  new: string;
};

export type CurrentUser = {
  displayName: string;
  emailAddress?: string;
  pictureUrl?: string;
};

export type EmailPayload = {
  address: string;
};

export type Modification<T> = {
  value?: T;
};

export type PhonePayload = {
  countryCode?: string;
  number: string;
  extension?: string;
};

export type SignInPayload = {
  uniqueName: string;
  password: string;
  isPersistent: boolean;
};

export type UpdateProfilePayload = {
  password?: ChangePasswordPayload;
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
};
