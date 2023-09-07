import type { Session } from "@/types/sessions";
import type { SignInPayload } from "@/types/sessions/payloads";
import type { UpdateUserPayload } from "@/types/users/payloads";
import type { User } from "@/types/users";
import { get, post, patch } from ".";

export async function getProfile(): Promise<User> {
  return (await get<User>("/api/account/profile")).data;
}

export async function saveProfile(payload: UpdateUserPayload): Promise<User> {
  return (await patch<UpdateUserPayload, User>("/api/account/profile", payload)).data;
}

export async function signIn(payload: SignInPayload): Promise<Session> {
  return (await post<SignInPayload, Session>("/api/account/sign/in", payload)).data;
}

export async function signOut(): Promise<void> {
  await post("/api/account/sign/out");
}
