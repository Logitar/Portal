import type { CreateTokenPayload, CreatedToken, ValidateTokenPayload, ValidatedToken } from "@/types/tokens";
import { post, put } from ".";

export async function createToken(payload: CreateTokenPayload): Promise<CreatedToken> {
  return (await post<CreateTokenPayload, CreatedToken>("/api/tokens", payload)).data;
}

export async function validateToken(payload: ValidateTokenPayload): Promise<ValidatedToken> {
  return (await put<ValidateTokenPayload, ValidatedToken>("/api/tokens", payload)).data;
}
