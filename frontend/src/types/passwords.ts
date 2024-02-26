import type { Aggregate } from "./aggregate";
import type { CustomAttribute } from "./customAttributes";
import type { Realm } from "./realms";

export type CreateOneTimePasswordPayload = {
  characters: string;
  length: number;
  expiresOn?: Date;
  maximumAttempts?: number;
  customAttributes: CustomAttribute[];
};

export type OneTimePassword = Aggregate & {
  password?: string;
  expiresOn?: Date;
  maximumAttempts?: number;
  attemptCount: number;
  hasValidationSucceeded: boolean;
  customAttributes: CustomAttribute[];
  realm?: Realm;
};

export type ValidateOneTimePasswordPayload = {
  password: string;
  customAttributes: CustomAttribute[];
};
