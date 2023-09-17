export type Claim = {
  name: string;
  value: string;
  type?: string;
};

export type CreateTokenPayload = {
  isConsumable: boolean;
  realm?: string;
  algorithm?: string;
  audience?: string;
  issuer?: string;
  lifetime: number;
  secret?: string;
  type?: string;
  subject?: string;
  emailAddress?: string;
  claims: Claim[];
};

export type CreatedToken = {
  token: string;
};

export type ValidateTokenPayload = {
  token: string;
  consume: boolean;
  realm?: string;
  audience?: string;
  issuer?: string;
  secret?: string;
  type?: string;
};

export type ValidatedToken = {
  subject?: string;
  emailAddress?: string;
  claims: Claim[];
};
