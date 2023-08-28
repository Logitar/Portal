import type { AddressPayload, EmailPayload, PhonePayload } from "./contact/payloads";

export type ChangePasswordPayload = {
  current: string;
  password: string;
};

export type SaveContactInformationPayload = {
  address?: AddressPayload;
  email: EmailPayload;
  phone?: PhonePayload;
};

export type SavePersonalInformationPayload = {
  firstName: string;
  middleName?: string;
  lastName: string;
  nickname?: string;
  birthdate?: Date;
  gender?: string;
  locale: string;
  timeZone?: string;
  picture?: string;
  profile?: string;
  website?: string;
};

export type SignInPayload = {
  username: string;
  password: string;
  remember: boolean;
};
