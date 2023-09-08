import type { CreateTokenPayload, CreatedToken, ValidateTokenPayload, ValidatedToken } from "@/types/tokens";
import { post } from ".";

export async function createToken(payload: CreateTokenPayload): Promise<CreatedToken> {
  return (await post<CreateTokenPayload, CreatedToken>("/api/tokens/create", payload)).data;
}

export async function validateToken(payload: ValidateTokenPayload): Promise<ValidatedToken> {
  return (await post<ValidateTokenPayload, ValidatedToken>("/api/tokens/validate", payload)).data;
}
